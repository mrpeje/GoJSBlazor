# GoJS-Alongside-Blazor


This sample project shows how to integrate [GoJS](https://gojs.net/latest/samples/dynamicPorts.html) in a [Blazor](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor) project using ASP.NET Core Blazor JavaScript interoperability (JS interop).

GoJsWrapper is a Razor component that integrates the GoJS JavaScript library for creating interactive diagrams within the Blazor environment. The project structure reflects GoJS concepts (diagrams, nodes, links, etc.), allowing  visualization of data directly in Blazor applications.
The crucial part of the project lies in the JavaScript file gojs-scripts.js, which connects GoJS with the Blazor component via JavaScript interop, enabling interaction between C# code and GoJS.

Key Features:
- Create/Edit blocks with ports on Palette and Diagram
- Connect block's ports by links
- Bind .Net events and Js events

[GoJSBlazor](https://mrpeje.github.io/) - This example demonstrates how to create diagrams using GoJS within Blazor applications. 
