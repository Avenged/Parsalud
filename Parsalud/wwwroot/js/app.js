window.loadLazyCss = (cssUrl, elementId) => {
    if (!document.getElementById(elementId)) {
        let link = document.createElement("link");
        link.id = elementId;
        link.rel = "stylesheet";
        link.href = cssUrl;
        document.head.appendChild(link);
    }
};