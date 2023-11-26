new function (self) {
    this.self = self;

    this.awake = function () {
        UnityEngine.Debug.Log('Awake', self);
    }

    this.start = function () {
        UnityEngine.Debug.Log('Start', self);
    }
    
    this.onDestroy = function () {
        UnityEngine.Debug.Log('OnDestroy', self);
    }

    this.onEnable = function () {
        UnityEngine.Debug.Log('OnEnable', self);
    }

    this.onDisable = function () {
        UnityEngine.Debug.Log('OnDisable', self);
    }
    
    this.update = function () {
        //UnityEngine.Debug.Log('update', self);
    }
    
    this.fixedUpdate = function () {
        //UnityEngine.Debug.Log('fixedUpdate', self);
    }
    
    this.lateUpdate = function () {
        //UnityEngine.Debug.Log('lateUpdate', self);
    }
    
    this.onTriggerEnter = function (other) {
        UnityEngine.Debug.Log('onTriggerEnter', self);
    }
    
    this.onTriggerExit = function (other) {
        UnityEngine.Debug.Log('onTriggerExit', self);
    }
    
    this.onTriggerStay = function (other) {
        UnityEngine.Debug.Log('onTriggerStay', self);
    }
    
    this.onCollisionEnter = function (other) {
        UnityEngine.Debug.Log('onCollisionEnter', self);
    }
    
    this.onCollisionExit = function (other) {
        UnityEngine.Debug.Log('onCollisionExit', self);
    }
    
    this.onCollisionStay = function (other) {
        UnityEngine.Debug.Log('onCollisionStay', self);
    }
}(self);