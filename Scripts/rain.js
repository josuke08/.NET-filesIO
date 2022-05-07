let img;
function preload() {
    img = loadImage('data:image/jpeg;base64,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');
}

var nds = 1;
class dropy {
    constructor(x = 0, y = 0, z = 0, len = 0,) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.big = Math.floor(-(len / 1000) * z + len);
        this.speed = Math.floor(-0.008 * z + 10);
        this.fat = Math.floor(-0.002 * z + 5);
    }

    drop_dropy() {
        this.y += this.speed;
    }

    draw_dropy() {
        image(img, this.x, this.Y, this.big, this.big);
        this.drop_dropy();
    }
}

function TheRandomizer() {
    return new dropy(Math.floor(Math.random() * window.innerWidth),
        Math.floor(Math.random() * window.innerHeight / 10) * -10,
        Math.floor(Math.random() * 650),
        Math.floor(Math.random() * (400 - 200) + 200)
    );
}

class rain {
    constructor() {
        this.drops = [];
        for (var i = 0; i < nds; i++)
            this.drops.push(TheRandomizer());

    }

    draw_em() {
        for (var i = 0; i < nds; i++) {
            this.drops[i].draw_dropy();
            if (this.drops[i].y - this.drops[i].big > window.innerHeight)
                this.drops[i] = TheRandomizer();
        }
    }
}

var r;

function setup() {
    r = new rain();
}

function draw() {
    createCanvas(window.innerWidth, window.innerHeight);
    r.draw_em();
}