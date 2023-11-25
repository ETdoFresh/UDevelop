class Component {
    constructor() {
        Debug.Log('Constructor called');
    }

    onEnable() {
        Debug.Log('OnEnable called');
    }

    onDisable() {
        Debug.Log('OnDisable called');
    }

    update() {
        // Called every frame
    }
}

const component = new Component();
return component;