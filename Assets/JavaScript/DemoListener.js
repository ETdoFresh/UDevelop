new function (self) {
    this.self = self;
    const _this = this;
    const DemoEvent = AssemblyCSharp.GetType("DemoEvent");

    this.onEnable = function () {
        EventBus.AddListener<DemoEvent>(_this.onDemoEvent);
    }

    this.onDisable = function () {
        EventBus.RemoveListener<DemoEvent>(_this.onDemoEvent);
    }

    this.onDemoEvent = function (e) {
        let message = "[" + self.ScriptName + "] ";
        message += self.name;
        message += ' received DemoEvent.';
        message += ' Message: ' + e.message;
        UnityEngine.Debug.Log(message, self);
    }
}(self);