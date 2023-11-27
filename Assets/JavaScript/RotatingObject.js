new function () {
    this.data = {
        "speed": 100.0,
        "isRotating": true,
        "rigidbody": null
    }

    this.onEnable = function (gameObject, data) {
        UnityEngine.Debug.Log("RotatingObject enabled for " + gameObject.name + " with speed: " + data.speed);
    }

    this.update = function (gameObject, data) {
        const up = Vector3.up;
        const time = Time.deltaTime;
        const speed = data.speed;

        if (isNull(data.rigidbody)) {
            data.rigidbody = gameObject.GetComponent(Rigidbody);
            data.rigidbody = isNotNull(data.rigidbody) ? data.rigidbody : null;
        }
        
        if (data.isRotating) {
            gameObject.transform.Rotate(multiply(up, speed * time));
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            data.rigidbody.AddForce(multiply(up, 200));
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            gameObject.transform.position = Vector3.zero;
            gameObject.transform.rotation = Quaternion.identity;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            data.isRotating = !data.isRotating;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            new GameObject("NewGameObject");
        }
    }
}