new function() {
    this.data = {
        "speed": 2.0
    }

    this.onEnable = function (gameObject, data) {
        UnityEngine.Debug.Log("RotatingObject enabled for " + gameObject.name + " with speed: " + data.speed);
    }
}