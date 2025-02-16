class ParsaludButton extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: "open" });
        this.render();
    }

    static get observedAttributes() {
        return ["href", "label"];
    }

    attributeChangedCallback() {
        this.render();
    }

    render() {
        const href = this.getAttribute("href") || "#";
        const label = this.getAttribute("label") || "Contactar";
        const variant = this.getAttribute("variant") || "filled";
        this.shadowRoot.innerHTML = `
            <style>
                a {
                    display: flex;
                    text-decoration: none !important;
                    gap: 0.30rem;
                    align-items: center;
                    white-space: nowrap;
                    border-radius: 3.125rem;
                    color: #fff;
                    border: none;
                    ${variant == 'text' ? '' : 'background-color: #D124B8'};
                    ${variant == 'text' ? '' : 'padding: 0.5rem 1.5rem;'}
                    ${variant == 'text' ? 'color: #9e0085;' : ''}
                }
            </style>
            <a href="${href}">
                ${label}
                  ${
                    variant == 'text' ?
                    '<img class="more-link" src="east.svg" alt="Navegar" height="24" width="24">' :
                    '<button-arrow></button-arrow>'
                  }
            </a>
        `;
    }
}

customElements.define("parsalud-button", ParsaludButton);
