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
        this.shadowRoot.innerHTML = `
            <style>
                a {
                    display: flex;
                    text-decoration: none !important;
                    gap: 0.30rem;
                    align-items: center;
                    white-space: nowrap;
                    background-color: #D124B8;
                    border-radius: 3.125rem;
                    color: #fff;
                    padding: 0.5rem 1.5rem;
                    border: none;
                }
            </style>
            <a href="${href}">
                ${label}
                <button-arrow></button-arrow>
            </a>
        `;
    }
}

customElements.define("parsalud-button", ParsaludButton);
