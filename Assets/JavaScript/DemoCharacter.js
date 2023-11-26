name = 'DemoCharacter';
hp = 100;
mp = 100;
attack = 10;
defense = 10;
speed = 10;
isDead = false;

function onEnable() {
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

function onDisable() {
    var disableMessage = "[" + self.ScriptName + "] ";
    disableMessage += self.name;
    disableMessage += ' has been disabled.';
    UnityEngine.Debug.Log(disableMessage, self);
}

function update() {
    //UnityEngine.Debug.Log('update', self);
}

new function(self) {
    const _this = this;
    this.self = self;
    this.name = name;
    this.hp = hp;
    this.mp = mp;
    this.attack = attack;
    this.defense = defense;
    this.speed = speed;
    this.isDead = isDead;
    this.onEnable = onEnable;
    this.onDisable = onDisable;
    this.update = update;
}(self);