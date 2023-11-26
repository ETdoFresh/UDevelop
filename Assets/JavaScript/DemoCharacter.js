new function (self) {
    this.self = self;
    var _this = this;

    this.name = 'DemoCharacter';
    this.hp = 100;
    this.mp = 100;
    this.attack = 10;
    this.defense = 10;
    this.speed = 10;
    this.isDead = false;

    this.onEnable = function () {
        var enableMessage = "[" + self.ScriptName + "] ";
        enableMessage += self.name;
        enableMessage += ' has been enabled.';
        enableMessage += ' Name: ' + _this.name;
        enableMessage += ' HP: ' + _this.hp;
        enableMessage += ' MP: ' + _this.mp;
        enableMessage += ' Attack: ' + _this.attack;
        enableMessage += ' Defense: ' + _this.defense;
        enableMessage += ' Speed: ' + _this.speed;
        UnityEngine.Debug.Log(enableMessage, self);
    }

    this.onDisable = function () {
        var disableMessage = "[" + self.ScriptName + "] ";
        disableMessage += self.name;
        disableMessage += ' has been disabled.';
        UnityEngine.Debug.Log(disableMessage, self);
    }

    this.update = function () {
        //UnityEngine.Debug.Log('update', self);
    }
}(self);