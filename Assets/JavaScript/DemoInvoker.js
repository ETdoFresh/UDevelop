new function (self) {
    this.self = self;
    const DemoEvent = AssemblyCSharp.GetType("DemoEvent");

    this.onEnable = function () {
        const demoEvent = new DemoEvent();
        demoEvent.message = 'Hello, World!';
        EventBus.Invoke<DemoEvent>(demoEvent);
        
        const camera = UnityEngine.Object.FindObjectOfType(UnityEngine.Camera)
        UnityEngine.Debug.Log("Found Camera: " + camera.name, camera);
    }
}(self);