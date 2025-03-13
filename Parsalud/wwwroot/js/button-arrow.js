class ButtonArrow extends HTMLElement {
    constructor() {
        super();
        this.attachShadow({ mode: "open" });
        this.render();
    }

    static get observedAttributes() {
        return ["color"];
    }

    attributeChangedCallback() {
        this.render();
    }

    render() {
        let color = this.getAttribute("color");

        if (color === '{Color}') {
            color = null;
        }

        this.shadowRoot.innerHTML = `
            <style>
                :host {
                    display: inline-block;
                    cursor: pointer;
                    width: 24px;
                    height: 24px;
                }
                svg {
                    width: 100%;
                    height: 100%;
                    ${color ? `color: ${color};` : ''}
                }
            </style>
            <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <mask id="mask0_9687_204" style="mask-type:alpha" maskUnits="userSpaceOnUse" x="0" y="0" width="24" height="24">
                    <rect width="24" height="24" fill="#D9D9D9"></rect>
                </mask>
                <g mask="url(#mask0_9687_204)">
                    <path d="M11.9998 20.2001L10.0998 18.35L15.1248 13.325H3.7998V10.675H15.1248L10.0998 5.65005L11.9998 3.80005L20.1998 12L11.9998 20.2001Z" fill="white"></path>
                </g>
            </svg>
        `;
    }
}

customElements.define("button-arrow", ButtonArrow);