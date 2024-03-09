    var netRefreneceVar = 1;

function initDiagram(netReference) {

        // Since 2.2 you can also author concise templates with method chaining instead of GraphObject.make
        // For details, see https://gojs.net/latest/intro/buildingObjects.html
        const $ = go.GraphObject.make;  //for conciseness in defining node templates
    
        myDiagram = $(go.Diagram, "myDiagramDiv",  // create a Diagram for the DIV HTML element
            {
                "undoManager.isEnabled": true  // enable undo & redo
            });

        // when the document is modified, add a "*" to the title and enable the "Save" button
        myDiagram.addDiagramListener("Modified", e => {
            const button = document.getElementById("SaveButton");
            if (button) button.disabled = !myDiagram.isModified;
            const idx = document.title.indexOf("*");
            if (myDiagram.isModified) {
                if (idx < 0) document.title += "*";
            } else {
                if (idx >= 0) document.title = document.title.slice(0, idx);
            }
        });
        const nodeMenu =  // context menu for each Node
            $("ContextMenu",
                makeButton("Copy",
                    (e, obj) => e.diagram.commandHandler.copySelection()),
                makeButton("Delete",
                    (e, obj) => e.diagram.commandHandler.deleteSelection()),
                $(go.Shape, "LineH", { strokeWidth: 2, height: 1, stretch: go.GraphObject.Horizontal }),
                makeButton("Add left port",
                    (e, obj) => addPort("left")),
                makeButton("Add right port",
                    (e, obj) => addPort("right")),
            );

        const portSize = new go.Size(8, 8);

        const portMenu =  // context menu for each port
            $("ContextMenu",
                makeButton("Swap order",
                    (e, obj) => swapOrder(obj.part.adornedObject)),
                makeButton("Remove port",
                    // in the click event handler, the obj.part is the Adornment;
                    // its adornedObject is the port
                    (e, obj) => removePort(obj.part.adornedObject)),
                makeButton("Change color",
                    (e, obj) => changeColor(obj.part.adornedObject)),
                makeButton("Remove side ports",
                    (e, obj) => removeAll(obj.part.adornedObject))
            );
        // To simplify this code we define a function for creating a context menu button:
        function makeButton(text, action, visiblePredicate) {
            return $("ContextMenuButton",
                $(go.TextBlock, text),
                { click: action },
                // don't bother with binding GraphObject.visible if there's no predicate
                visiblePredicate ? new go.Binding("visible", "", (o, e) => o.diagram ? visiblePredicate(o, e) : false).ofObject() : {});
        }
        function makeToolTip(title) {
            var $ = go.GraphObject.make;
            return $("ToolTip",
                $(go.TextBlock, { margin: 4 },
                    new go.Binding("text", "description"))
            ) 
        }
        function createNode() { return  $(go.Node, "Table",
                {
                    locationObjectName: "BODY",
                    locationSpot: go.Spot.Center,
                    selectionObjectName: "BODY",
                    contextMenu: nodeMenu,
                    toolTip: makeToolTip()
                },
                new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),  
                new go.Binding("text", "description"),
                { 
                    mouseHover: (e, obj) => {
                        netReference.invokeMethodAsync('OnNodeMouseHoverEvent', obj.key.toString());
                    }
                },
                // the body
                $(go.Panel, "Auto",
                    {
                        row: 1, column: 1, name: "BODY",
                        stretch: go.GraphObject.Fill
                    },
                    $(go.Shape, "Rectangle",
                        {
                            fill: "#dbf6cb", stroke: null, strokeWidth: 0,
                            minSize: new go.Size(60, 60)
                        },
                        new go.Binding("fill", "color")),
                    $(go.TextBlock,
                        { margin: 10, textAlign: "center", font: "bold 14px Segoe UI,sans-serif", stroke: "#484848", editable: true },
                        new go.Binding("text", "name").makeTwoWay())
                ),  // end Auto Panel body

                // the Panel holding the left port elements, which are themselves Panels,
                // created for each item in the itemArray, bound to data.leftArray
                $(go.Panel, "Vertical",
                    new go.Binding("itemArray", "leftArray"),
                    {
                        row: 1, column: 0,
                        itemTemplate:
                            $(go.Panel,
                                {
                                    _side: "left",  // internal property to make it easier to tell which side it's on
                                    fromSpot: go.Spot.Left, toSpot: go.Spot.Left,
                                    fromLinkable: true, toLinkable: true, cursor: "pointer",
                                    contextMenu: portMenu,
                                    toolTip: makeToolTip()
                                },
                                new go.Binding("portId", "portId"),
                                
                                $(go.Shape, "Rectangle",
                                    {
                                        stroke: null, strokeWidth: 0,
                                        desiredSize: portSize,
                                        margin: new go.Margin(1, 0)
                                    },
                                    new go.Binding("fill", "portColor"),
                                    new go.Binding("text", "description")
                                )
                            )  // end itemTemplate
                    }
                ),  // end Vertical Panel



                // the Panel holding the right port elements, which are themselves Panels,
                // created for each item in the itemArray, bound to data.rightArray
                $(go.Panel, "Vertical",
                    new go.Binding("itemArray", "rightArray"),
                    {
                        row: 1, column: 2,
                        itemTemplate:
                            $(go.Panel,
                                {
                                    _side: "right",
                                    fromSpot: go.Spot.Right, toSpot: go.Spot.Right,
                                    fromLinkable: true, toLinkable: true, cursor: "pointer",
                                    contextMenu: portMenu,
                                    toolTip: makeToolTip()
                                },
                                new go.Binding("portId", "portId"),
                                new go.Binding("text", "description"),
                                $(go.Shape, "Rectangle",
                                    {
                                        stroke: null, strokeWidth: 0,
                                        desiredSize: portSize,
                                        margin: new go.Margin(1, 0)
                                    },
                                    new go.Binding("fill", "portColor"))
                            )  // end itemTemplate
                    }
                ),  // end Vertical Panel

                // end Horizontal Panel
        )
        }

        // the node template
        // includes a panel on each side with an itemArray of panels containing ports
        myDiagram.nodeTemplate = createNode();  // end Node
   
        // an orthogonal link template, reshapable and relinkable
        myDiagram.linkTemplate =
            $(CustomLink,  // defined below
                {
                    routing: go.Link.AvoidsNodes,
                    corner: 4,
                    curve: go.Link.JumpGap,
                    reshapable: true,
                    resegmentable: true,
                    relinkableFrom: true,
                    relinkableTo: true
                },
                {
                    mouseHover: (e, obj) => {
                        netReference.invokeMethodAsync('OnLinkMouseHoverEvent', obj.key.toString());
                    }
                },
                new go.Binding("points").makeTwoWay(),
                $(go.Shape, { stroke: "#2F4F4F", strokeWidth: 2 })
            );
    

        // support double-clicking in the background to add a copy of this data as a node
        myDiagram.toolManager.clickCreatingTool.archetypeNodeData = {
            name: "Unit",
            leftArray: [],
            rightArray: []
        };

        myDiagram.contextMenu =
            $("ContextMenu",
                makeButton("Paste",
                    (e, obj) => e.diagram.commandHandler.pasteSelection(e.diagram.toolManager.contextMenuTool.mouseDownPoint),
                    o => o.diagram.commandHandler.canPasteSelection(o.diagram.toolManager.contextMenuTool.mouseDownPoint)),
                makeButton("Undo",
                    (e, obj) => e.diagram.commandHandler.undo(),
                    o => o.diagram.commandHandler.canUndo()),
                makeButton("Redo",
                    (e, obj) => e.diagram.commandHandler.redo(),
                    o => o.diagram.commandHandler.canRedo())
            );

        myPalette =
            new go.Palette("myPaletteDiv",
                {
                    nodeTemplateMap: myDiagram.nodeTemplateMap, 
                    linkTemplate: myDiagram.linkTemplate

                });

        // load the diagram from JSON data
        load();  
    }

    // This custom-routing Link class tries to separate parallel links from each other.
    // This assumes that ports are lined up in a row/column on a side of the node.
    class CustomLink extends go.Link {
        findSidePortIndexAndCount(node, port) {
            const nodedata = node.data;
            if (nodedata !== null) {
                const portdata = port.data;
                const side = port._side;
                const arr = nodedata[side + "Array"];
                const len = arr.length;
                for (let i = 0; i < len; i++) {
                    if (arr[i] === portdata) return [i, len];
                }
            }
            return [-1, len];
        }

        computeEndSegmentLength(node, port, spot, from) {
            const esl = super.computeEndSegmentLength(node, port, spot, from);
            const other = this.getOtherPort(port);
            if (port !== null && other !== null) {
                const thispt = port.getDocumentPoint(this.computeSpot(from));
                const otherpt = other.getDocumentPoint(this.computeSpot(!from));
                if (Math.abs(thispt.x - otherpt.x) > 20 || Math.abs(thispt.y - otherpt.y) > 20) {
                    const info = this.findSidePortIndexAndCount(node, port);
                    const idx = info[0];
                    const count = info[1];
                    if (port._side == "top" || port._side == "bottom") {
                        if (otherpt.x < thispt.x) {
                            return esl + 4 + idx * 8;
                        } else {
                            return esl + (count - idx - 1) * 8;
                        }
                    } else {  // left or right
                        if (otherpt.y < thispt.y) {
                            return esl + 4 + idx * 8;
                        } else {
                            return esl + (count - idx - 1) * 8;
                        }
                    }
                }
            }
            return esl;
        }

        hasCurviness() {
            if (isNaN(this.curviness)) return true;
            return super.hasCurviness();
        }

        computeCurviness() {
            if (isNaN(this.curviness)) {
                const fromnode = this.fromNode;
                const fromport = this.fromPort;
                const fromspot = this.computeSpot(true);
                const frompt = fromport.getDocumentPoint(fromspot);
                const tonode = this.toNode;
                const toport = this.toPort;
                const tospot = this.computeSpot(false);
                const topt = toport.getDocumentPoint(tospot);
                if (Math.abs(frompt.x - topt.x) > 20 || Math.abs(frompt.y - topt.y) > 20) {
                    if ((fromspot.equals(go.Spot.Left) || fromspot.equals(go.Spot.Right)) &&
                        (tospot.equals(go.Spot.Left) || tospot.equals(go.Spot.Right))) {
                        const fromseglen = this.computeEndSegmentLength(fromnode, fromport, fromspot, true);
                        const toseglen = this.computeEndSegmentLength(tonode, toport, tospot, false);
                        const c = (fromseglen - toseglen) / 2;
                        if (frompt.x + fromseglen >= topt.x - toseglen) {
                            if (frompt.y < topt.y) return c;
                            if (frompt.y > topt.y) return -c;
                        }
                    } else if ((fromspot.equals(go.Spot.Top) || fromspot.equals(go.Spot.Bottom)) &&
                        (tospot.equals(go.Spot.Top) || tospot.equals(go.Spot.Bottom))) {
                        const fromseglen = this.computeEndSegmentLength(fromnode, fromport, fromspot, true);
                        const toseglen = this.computeEndSegmentLength(tonode, toport, tospot, false);
                        const c = (fromseglen - toseglen) / 2;
                        if (frompt.x + fromseglen >= topt.x - toseglen) {
                            if (frompt.y < topt.y) return c;
                            if (frompt.y > topt.y) return -c;
                        }
                    }
                }
            }
            return super.computeCurviness();
        }
    }
    // end CustomLink class


    // Add a port to the specified side of the selected nodes.
    function addPort(side) {
        myDiagram.startTransaction("addPort");
        myDiagram.selection.each(node => {
            // skip any selected Links
            if (!(node instanceof go.Node)) return;
            // compute the next available index number for the side
            let i = 0;
            while (node.findPort(side + i.toString()) !== node) i++;
            // now this new port name is unique within the whole Node because of the side prefix
            const name = side + i.toString();
            // get the Array of port data to be modified
            const arr = node.data[side + "Array"];
            if (arr) {
                // create a new port data object
                const newportdata = {
                    portId: name,
                    description: "Port description",
                    PortName: "Port name",
                    portColor: getPortColor()
                };
                // and add it to the Array of port data
                myDiagram.model.insertArrayItem(arr, -1, newportdata);
            }
        });
        myDiagram.commitTransaction("addPort");
    }

    // Exchange the position/order of the given port with the next one.
    // If it's the last one, swap with the previous one.
    function swapOrder(port) {
        const arr = port.panel.itemArray;
        if (arr.length >= 2) {  // only if there are at least two ports!
            for (let i = 0; i < arr.length; i++) {
                if (arr[i].portId === port.portId) {
                    myDiagram.startTransaction("swap ports");
                    if (i >= arr.length - 1) i--;  // now can swap I and I+1, even if it's the last port
                    const newarr = arr.slice(0);  // copy Array
                    newarr[i] = arr[i + 1];  // swap items
                    newarr[i + 1] = arr[i];
                    // remember the new Array in the model
                    myDiagram.model.setDataProperty(port.part.data, port._side + "Array", newarr);
                    port.part.findLinksConnected(newarr[i].portId).each(l => l.invalidateRoute());
                    port.part.findLinksConnected(newarr[i + 1].portId).each(l => l.invalidateRoute());
                    myDiagram.commitTransaction("swap ports");
                    break;
                }
            }
        }
    }

    // Remove the clicked port from the node.
    // Links to the port will be redrawn to the node's shape.
    function removePort(port) {
        myDiagram.startTransaction("removePort");
        const pid = port.portId;
        const arr = port.panel.itemArray;
        for (let i = 0; i < arr.length; i++) {
            if (arr[i].portId === pid) {
                myDiagram.model.removeArrayItem(arr, i);
                break;
            }
        }
        myDiagram.commitTransaction("removePort");
    }

    // Remove all ports from the same side of the node as the clicked port.
    function removeAll(port) {
        myDiagram.startTransaction("removePorts");
        const nodedata = port.part.data;
        const side = port._side;  // there are four property names, all ending in "Array"
        myDiagram.model.setDataProperty(nodedata, side + "Array", []);  // an empty Array
        myDiagram.commitTransaction("removePorts");
    }

    // Change the color of the clicked port.
    function changeColor(port) {
        myDiagram.startTransaction("colorPort");
        const data = port.data;
        myDiagram.model.setDataProperty(data, "portColor", getPortColor());
        myDiagram.commitTransaction("colorPort");
    }

    // Use some pastel colors for ports
    function getPortColor() {
        const portColors = ["#fae3d7", "#d6effc", "#ebe3fc", "#eaeef8", "#fadfe5", "#6cafdb", "#66d6d1"]
        return portColors[Math.floor(Math.random() * portColors.length)];
    }

    // Save the model to / load it from JSON text shown on the page itself, not in a database.
    function save() {
        var jsonModel = myDiagram.model.toJson();
        document.getElementById("mySavedModel").value = jsonModel;
        myDiagram.isModified = false;
    }
    function getModelJson() {
        return myDiagram.model.toJson();
    }
    function load() {

        var modelJson = jsonDefault;
        myDiagram.model = go.Model.fromJson(modelJson);
    }

    function subscribeSelectionChangedEventListener(dotNetObjectRef) {
        myDiagram.addDiagramListener('ChangedSelection', (event) => {
            var obj = event.diagram.selection.first();
            if (obj instanceof go.Node) {
                dotNetObjectRef.invokeMethodAsync('OnNodeSelectionChangedEvent', obj.key.toString());
            }
            if (obj instanceof go.Link) {
                dotNetObjectRef.invokeMethodAsync('OnLinkSelectionChangedEvent', obj.key.toString());
            }
        });
    }
    function subscribeModelChangedEvent(netRefrenece) {
        myDiagram.addModelChangedListener(evt => {
            if (evt.isTransactionFinished)
                netRefrenece.invokeMethod('OnDiagramModelChangedEvent', evt.model.toJson());
        });
        myPalette.addModelChangedListener(evt => {
            if (evt.isTransactionFinished)
                netRefrenece.invokeMethod('OnPaletteModelChangedEvent', myPalette.model.toJson());
        });
    }

function subscribeBlockMovedEvent(netRefreneceBlockPositionChanged) {
        myDiagram.addModelChangedListener(evt => {
            if (!evt.isTransactionFinished) return;
            var txn = evt.object; 
            if (txn === null) return;
            txn.changes.each(e => { 
                //if (e.modelChange !== "nodeDataArray") return;
                if (e.change === go.ChangedEvent.Property) {
                    if (e.propertyName === "loc")
                        netRefreneceBlockPositionChanged.invokeMethod('OnBlockPositionChangedEvent', e.object.name );
                }
            });
        });
    }



    function removeBlock(block) {
        var jsonBlock = JSON.parse(block);
        myDiagram.startTransaction("Delete new block");
        var node = myDiagram.model.findNodeDataForKey(jsonBlock.key);
        myDiagram.model.removeNodeData(node);
        myDiagram.commitTransaction("Deletenewblock");
    }
    function addNewBlock(newBlock) {
        var jsonNewBlock = JSON.parse(newBlock);
        myDiagram.startTransaction("makenewblock");
        myDiagram.model.addNodeData(jsonNewBlock);
        myDiagram.commitTransaction("makenewblock");
    }
    function addNewPaletteBlock(newBlock) {
        var jsonNewBlock = JSON.parse(newBlock);
        myPalette.startTransaction("make new block on palette");
        myPalette.model.addNodeData(jsonNewBlock);
        myPalette.commitTransaction("make new block on palette");
    }
    function removePaletteBlock(blockId) {
        myPalette.startTransaction("Delete new block");
        var node = myPalette.model.findNodeDataForKey(blockId);
        myPalette.model.removeNodeData(node);
        myPalette.commitTransaction("Delete new block");
    }
    function updateBlock(block) {
        var jsonBlock = JSON.parse(block);
        var node = myDiagram.model.findNodeDataForKey(jsonBlock.key);
        myDiagram.startTransaction("update block");
        myDiagram.model.setDataProperty(node, "color", jsonBlock.color);
        myDiagram.model.setDataProperty(node, "name", jsonBlock.name);
        myDiagram.model.setDataProperty(node, "leftArray", jsonBlock.leftArray);
        myDiagram.model.setDataProperty(node, "rightArray", jsonBlock.rightArray);
        myDiagram.model.setDataProperty(node, "description", jsonBlock.description);
        myDiagram.commitTransaction("update block");
    }
    function updateBlockPosition(blockKey, newCoordinates) {

        var node = myDiagram.model.findNodeDataForKey(blockKey);
        myDiagram.startTransaction("update block position");
        myDiagram.model.setDataProperty(node, "loc", newCoordinates);
        myDiagram.commitTransaction("update block position");
    }
    function addLink(newLink) {
        var jsonNewLink = JSON.parse(newLink);
        myDiagram.startTransaction("makenewlink");
        myDiagram.model.addLinkData(jsonNewLink);;
        myDiagram.commitTransaction("makenewlink");  
    }

    function deleteLink(link) {
        var jsonNewLink = JSON.parse(link);
        myDiagram.startTransaction("remove link");
        var linkdata = myDiagram.model.findLinkDataForKey(jsonNewLink.key);
        myDiagram.model.removeLinkData(linkdata);
        myDiagram.commitTransaction("remove link");
    }
    //function load() {

    //    myDiagram.model = go.Model.fromJson(document.getElementById("mySavedModel").value);

    //    // When copying a node, we need to copy the data that the node is bound to.
    //    // This JavaScript object includes properties for the node as a whole, and
    //    // four properties that are Arrays holding data for each port.
    //    // Those arrays and port data objects need to be copied too.
    //    // Thus Model.copiesArrays and Model.copiesArrayObjects both need to be true.

    //    // Link data includes the names of the to- and from- ports;
    //    // so the GraphLinksModel needs to set these property names:
    //    // linkFromPortIdProperty and linkToPortIdProperty.
//}
const jsonDefault = {
    "class": "go.GraphLinksModel",
    "linkKeyProperty": 'key',
    "copiesArrays": true,
    "copiesArrayObjects": true,
    "linkFromPortIdProperty": "fromPort",
    "linkToPortIdProperty": "toPort",
    "nodeDataArray": [
        {
            "key": 1,"category": "Cat1","description":"asd", "name": "Unit One", "loc": "101 204", "color": "#66d6d1",
            "leftArray": [{ "portColor": "#fae3d7", "portId": "left0", "description": "asdasd" }],
            "rightArray": [{ "portColor": "#eaeef8", "portId": "right0" }, { "portColor": "#fadfe5", "portId": "right1" }]
        }  
    ],
    "linkDataArray": [
       
    ]
}
    const jsonDefault1 = {
        "class": "go.GraphLinksModel",
        "linkKeyProperty": 'key',
        "copiesArrays": true,
        "copiesArrayObjects": true,
        "linkFromPortIdProperty": "fromPort",
        "linkToPortIdProperty": "toPort",
        "nodeDataArray": [
            {
                "key": 1, "name": "Unit One", "loc": "101 204", "color": "#66d6d1",
                "leftArray": [{ "portColor": "#fae3d7", "portId": "left0" }],          
                "rightArray": [{ "portColor": "#eaeef8", "portId": "right0" }, { "portColor": "#fadfe5", "portId": "right1" }]
            },
            {
                "key": 2, "name": "Unit Two", "loc": "320 152",
                "leftArray": [{ "portColor": "#6cafdb", "portId": "left0" }, { "portColor": "#66d6d1", "portId": "left1" }, { "portColor": "#fae3d7", "portId": "left2" }],
                "rightArray": []
            },
            {
                "key": 3, "name": "Unit Three", "loc": "384 319",
                "leftArray": [{ "portColor": "#66d6d1", "portId": "left0" }, { "portColor": "#fadfe5", "portId": "left1" }, { "portColor": "#6cafdb", "portId": "left2" }],
                "rightArray": []
            },
            {
                "key": 4, "name": "Unit Four", "loc": "138 351",
                "leftArray": [{ "portColor": "#fae3d7", "portId": "left0" }],
                "rightArray": [{ "portColor": "#6cafdb", "portId": "right0" }, { "portColor": "#66d6d1", "portId": "right1" }]
            }
        ],
        "linkDataArray": [
            { "from": 4, "to": 3, "fromPort": "right0", "toPort": "left0" },
            { "from": 4, "to": 3, "fromPort": "right1", "toPort": "left2" },
            { "from": 1, "to": 2, "fromPort": "right0", "toPort": "left1" },
            { "from": 1, "to": 2, "fromPort": "right1", "toPort": "left2" }
        ]
    }

    /*window.addEventListener('DOMContentLoaded', initDiagram);*/