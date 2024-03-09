function init() {

    // Since 2.2 you can also author concise templates with method chaining instead of GraphObject.make
    // For details, see https://gojs.net/latest/intro/buildingObjects.html
    const $ = go.GraphObject.make;  //for conciseness in defining node templates

    myDiagram =
        new go.Diagram("myDiagramDiv",  //Diagram refers to its DIV HTML element by id
            { "undoManager.isEnabled": true });

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

    // To simplify this code we define a function for creating a context menu button:
    function makeButton(text, action, visiblePredicate) {
        return $("ContextMenuButton",
            $(go.TextBlock, text),
            { click: action },
            // don't bother with binding GraphObject.visible if there's no predicate
            visiblePredicate ? new go.Binding("visible", "", (o, e) => o.diagram ? visiblePredicate(o, e) : false).ofObject() : {});
    }

    const nodeMenu =  // context menu for each Node
        $("ContextMenu",
            makeButton("Copy",
                (e, obj) => e.diagram.commandHandler.copySelection()),
            makeButton("Delete",
                (e, obj) => e.diagram.commandHandler.deleteSelection()),
            $(go.Shape, "LineH", { strokeWidth: 2, height: 1, stretch: go.GraphObject.Horizontal }),
            makeButton("Add top port",
                (e, obj) => addPort("top")),
            makeButton("Add left port",
                (e, obj) => addPort("left")),
            makeButton("Add right port",
                (e, obj) => addPort("right")),
            makeButton("Add bottom port",
                (e, obj) => addPort("bottom"))
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

    // the node template
    // includes a panel on each side with an itemArray of panels containing ports
    myDiagram.nodeTemplate =
        $(go.Node, "Table",
            {
                locationObjectName: "BODY",
                locationSpot: go.Spot.Center,
                selectionObjectName: "BODY",
                contextMenu: nodeMenu
            },
            new go.Binding("location", "loc", go.Point.parse).makeTwoWay(go.Point.stringify),

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
                    }),
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
                                contextMenu: portMenu
                            },
                            new go.Binding("portId", "portId"),
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

            // the Panel holding the top port elements, which are themselves Panels,
            // created for each item in the itemArray, bound to data.topArray
            $(go.Panel, "Horizontal",
                new go.Binding("itemArray", "topArray"),
                {
                    row: 0, column: 1,
                    itemTemplate:
                        $(go.Panel,
                            {
                                _side: "top",
                                fromSpot: go.Spot.Top, toSpot: go.Spot.Top,
                                fromLinkable: true, toLinkable: true, cursor: "pointer",
                                contextMenu: portMenu
                            },
                            new go.Binding("portId", "portId"),
                            $(go.Shape, "Rectangle",
                                {
                                    stroke: null, strokeWidth: 0,
                                    desiredSize: portSize,
                                    margin: new go.Margin(0, 1)
                                },
                                new go.Binding("fill", "portColor"))
                        )  // end itemTemplate
                }
            ),  // end Horizontal Panel

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
                                contextMenu: portMenu
                            },
                            new go.Binding("portId", "portId"),
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

            // the Panel holding the bottom port elements, which are themselves Panels,
            // created for each item in the itemArray, bound to data.bottomArray
            $(go.Panel, "Horizontal",
                new go.Binding("itemArray", "bottomArray"),
                {
                    row: 2, column: 1,
                    itemTemplate:
                        $(go.Panel,
                            {
                                _side: "bottom",
                                fromSpot: go.Spot.Bottom, toSpot: go.Spot.Bottom,
                                fromLinkable: true, toLinkable: true, cursor: "pointer",
                                contextMenu: portMenu
                            },
                            new go.Binding("portId", "portId"),
                            $(go.Shape, "Rectangle",
                                {
                                    stroke: null, strokeWidth: 0,
                                    desiredSize: portSize,
                                    margin: new go.Margin(0, 1)
                                },
                                new go.Binding("fill", "portColor"))
                        )  // end itemTemplate
                }
            )  // end Horizontal Panel
        );  // end Node

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
            new go.Binding("points").makeTwoWay(),
            $(go.Shape, { stroke: "#2F4F4F", strokeWidth: 2 })
        );

    // support double-clicking in the background to add a copy of this data as a node
    myDiagram.toolManager.clickCreatingTool.archetypeNodeData = {
        name: "Unit",
        leftArray: [],
        rightArray: [],
        topArray: [],
        bottomArray: []
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

    // load the diagram from JSON data
    //load();
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
    document.getElementById("mySavedModel").value = myDiagram.model.toJson();
    myDiagram.isModified = false;
}
function load() {
    myDiagram.model = go.Model.fromJson(document.getElementById("mySavedModel").value);

    // When copying a node, we need to copy the data that the node is bound to.
    // This JavaScript object includes properties for the node as a whole, and
    // four properties that are Arrays holding data for each port.
    // Those arrays and port data objects need to be copied too.
    // Thus Model.copiesArrays and Model.copiesArrayObjects both need to be true.

    // Link data includes the names of the to- and from- ports;
    // so the GraphLinksModel needs to set these property names:
    // linkFromPortIdProperty and linkToPortIdProperty.
}