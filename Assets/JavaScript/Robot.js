new function (self) {
    this.self = self;

    this.awake = function () {
        UnityEngine.Debug.Log('Awake');
    }

    this.start = function () {
        UnityEngine.Debug.Log('Start');
    }
    
    this.onDestroy = function () {
        UnityEngine.Debug.Log('OnDestroy');
    }

    this.onEnable = function () {
        UnityEngine.Debug.Log('OnEnable');
    }

    this.onDisable = function () {
        UnityEngine.Debug.Log('OnDisable');
    }
    
    this.update = function () {
        //UnityEngine.Debug.Log('update');
    }
    
    this.fixedUpdate = function () {
        //UnityEngine.Debug.Log('fixedUpdate');
    }
    
    this.lateUpdate = function () {
        //UnityEngine.Debug.Log('lateUpdate');
    }
    
    this.onTriggerEnter = function (other) {
        UnityEngine.Debug.Log('onTriggerEnter');
    }
    
    this.onTriggerExit = function (other) {
        UnityEngine.Debug.Log('onTriggerExit');
    }
    
    this.onTriggerStay = function (other) {
        UnityEngine.Debug.Log('onTriggerStay');
    }
    
    this.onCollisionEnter = function (other) {
        UnityEngine.Debug.Log('onCollisionEnter');
    }
    
    this.onCollisionExit = function (other) {
        UnityEngine.Debug.Log('onCollisionExit');
    }
    
    this.onCollisionStay = function (other) {
        UnityEngine.Debug.Log('onCollisionStay');
    }
}(self);