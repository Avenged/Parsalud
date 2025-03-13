class ParsaludButton extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: "open" });
        this.render();
    }

    static get observedAttributes() {
        return ["color", "classes", "href", "label", "target"];
    }

    attributeChangedCallback() {
        this.render();
    }

    render() {

        function buildButtonArrow(color) {
            return color ? `<button-arrow color="${color}"></button-arrow>` : '<button-arrow></button-arrow>'
        }

        function buildMoreLink() {
            return '<img class="more-link" src="east.svg" alt="Navegar" height="24" width="24">';
        }

        let color = this.getAttribute("color");
        const classes = this.getAttribute("class") || "#";
        const href = this.getAttribute("href") || "#";
        const target = this.getAttribute("target") || "_self";
        const label = this.getAttribute("label") || "Contactar";
        const variant = this.getAttribute("variant") || "filled";

        if (color === '{Color}') {
            color = null;
        }

        this.shadowRoot.innerHTML = `
            <style>
                a {
                    width: min-content;
                    display: flex;
                    text-decoration: none !important;
                    gap: 0.30rem;
                    align-items: center;
                    white-space: nowrap;
                    border-radius: 3.125rem;
                    color: #fff;
                    border: none;
                    ${variant == 'text' ? '' : 'background-color: #D124B8;'}
                    ${variant == 'text' ? '' : 'padding: 0.5rem 1.5rem;'}
                    ${variant == 'text' ? 'color: #9e0085;' : ''}
                    ${color && `color: ${color}`}
                }
            </style>
            <a class="${classes}" href="${href}" target="${target}">
                ${label}
                  ${
                    variant == 'text' && !color ? buildMoreLink() : buildButtonArrow(color)
                  }
            </a>
        `;
    }
}

customElements.define("parsalud-button", ParsaludButton);
