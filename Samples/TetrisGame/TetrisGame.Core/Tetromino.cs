using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
    var Tetrimino = ctx.Tetrimino = cc.Sprite.extend({

    ctor: function(fallSpeed) {

       this._super();
      this.fallSpeed = fallSpeed;// interval in seconds
      this.timeToFall = this.fallSpeed;
      this.setContentSize(BrickSprite.WIDTH* Tetrimino.SIZE, BrickSprite.HEIGHT* Tetrimino.SIZE);
      this.grid = 0;
      this.gridPos = { x: 0, y: 0};
      this.bricksPlan = Utils.getRandomItem(Tetrimino.PLANS);
      this.rotationInd = Utils.getRandomInt(this.bricksPlan.length);
      this.bricksMap = [];
      this.action = Tetrimino.ACTION_NONE;
      this.isFrozen = false;
      this.isAccelerated = false;
    },

    update: function(dt)
    {
        if (cc.game.keyboardEvent) this.onKeyboardEvent(cc.game.keyboardEvent);
        switch (this.action)
        {
            case Tetrimino.ACTION_MOVE_LEFT: this.moveLeft(); break;
            case Tetrimino.ACTION_MOVE_RIGHT: this.moveRight(); break;
            case Tetrimino.ACTION_MOVE_DOWN: this.accelerate(); break;
            case Tetrimino.ACTION_ROTATE: this.rotate(); break;
            case Tetrimino.ACTION_STOP_MOVE: this.stopAcceleration(); break;
        }
        this.action = Tetrimino.ACTION_NONE;

        if (this.isAccelerated)
        {
            this.moveDown();
        }

        this.timeToFall -= dt;
        if (this.timeToFall <= 0)
        {
            this.timeToFall = this.fallSpeed;
            if (this.canMoveDown())
            {
                this.moveDown();
            }
            else
            {
          $clickSound.play();
                this.freeze();
            }
        }
    },

    moveRight: function()
    {
        var newPos = { x: this.gridPos.x + 1, y: this.gridPos.y};
      if (this.isValidPosition(newPos)) this.setGridPos(newPos);
      $clickSound.play();
    },

    moveLeft: function()
    {
        var newPos = { x: this.gridPos.x - 1, y: this.gridPos.y};
      if (this.isValidPosition(newPos)) this.setGridPos(newPos);
      $clickSound.play();
    },

    moveDown: function()
    {
        if (this.canMoveDown())
        {
            this.setGridPos({ y: this.gridPos.y - 1});
        $clickSound.play();
        }
    },

    canMoveDown: function()
    {
        return this.isValidPosition({ x: this.gridPos.x, y: this.gridPos.y - 1});
    },

    accelerate: function()
    {
        this.isAccelerated = true;
    },

    stopAcceleration: function()
    {
        this.isAccelerated = false;
    },

    rotate: function()
    {
        var rotatedBricksMap = this.getRotatedBricksMap();
        var rotatedPaddings = Tetrimino.getPaddings(rotatedBricksMap);
        var canRotate = false;
        if (this.isValidPosition(this.gridPos, rotatedBricksMap))
        {
            canRotate = true;
        }
        else
        {
            // move tetrominto to left or to right if after rotation it across borders
            var leftLedge = -(this.gridPos.x + rotatedPaddings.left);
            var rightLedge = (this.gridPos.x + Tetrimino.SIZE - rotatedPaddings.right) - this.grid.size.width;
            var correctToRightPos = { x: this.gridPos.x + leftLedge, y: this.gridPos.y};
        var correctToLeftPos = { x: this.gridPos.x - rightLedge, y: this.gridPos.y};
        if (leftLedge > 0 && this.isValidPosition(correctToRightPos, rotatedBricksMap)) {
        this.setGridPos(correctToRightPos);
        canRotate = true;
    } else if (rightLedge > 0 && this.isValidPosition(correctToLeftPos, rotatedBricksMap)) {
        this.setGridPos(correctToLeftPos);
        canRotate = true;
    }
    }
      if (canRotate) {
        $clickSound.play();
        this.rotationInd = this._getNextRotationInd();
        this.render();
      }
},

    freeze: function() {
    this.isFrozen = true;
    this.grid.addBricksFromTetrimino(this);
},

    _getNextRotationInd: function() {
    var nextRotationInd = this.rotationInd + 1;
    return nextRotationInd < this.bricksPlan.length ? nextRotationInd : 0;
},

    getRotatedBricksMap: function() {
    return this.bricksPlan[this._getNextRotationInd()];
},

    setGridPos: function(gridPos) {
    this.gridPos.x = gridPos.x !== void 0 ? gridPos.x : this.gridPos.x;
    this.gridPos.y = gridPos.y !== void 0 ? gridPos.y : this.gridPos.y;
    this.setPosition(
      this.gridPos.x * BrickSprite.WIDTH + this.width / 2,
      this.gridPos.y * BrickSprite.HEIGHT + this.height / 2
    )
    },

    isValidPosition: function(gridPos, bricksMap) {
    bricksMap = bricksMap || this.bricksMap;
    var ri = Tetrimino.SIZE;
    while (ri--)
    {
        for (var ci = 0; ci < Tetrimino.SIZE; ci++)
        {
            if (!bricksMap[ri][ci]) continue;
            var brickPos = { x: gridPos.x + ci, y: gridPos.y + Tetrimino.SIZE - ri - 1};
        var isOutOfBounds = (
          brickPos.y < 0 || brickPos.x < 0 ||
          brickPos.x >= this.grid.size.width ||
          brickPos.y >= this.grid.size.height
        );
        if (isOutOfBounds) return false;
        var isCollidesBricks = this.grid.bricksMap[brickPos.y][brickPos.x];
        if (isCollidesBricks) return false;
    }
}
return true;
    },

    onEnter: function() {
    this.render();
    this.setGridPos({ x: 3, y: 16 + this.getPaddings().top});
},

    render: function() {
    this.removeAllChildren();
    this.bricksMap = this.bricksPlan[this.rotationInd];
    var rowInd = Tetrimino.SIZE;
    while (rowInd--)
    {
        for (var colInd = 0; colInd < Tetrimino.SIZE; colInd++)
        {
            if (!this.bricksMap[rowInd][colInd]) continue;
            var brick = new BrickSprite();
            brick.setPosition(
                colInd * BrickSprite.WIDTH + BrickSprite.WIDTH / 2,
                (Tetrimino.SIZE - rowInd - 1) * BrickSprite.HEIGHT
            );
            this.addChild(brick);
        }
    }
},

    onKeyboardEvent: function(ev) {
    if (ev.type === 'keyup')
    {
        if (ev.code === 'ArrowDown') this.action = Tetrimino.ACTION_STOP_MOVE;
        return;
    }
    switch (ev.code)
    {
        case 'ArrowRight': this.action = Tetrimino.ACTION_MOVE_RIGHT; break;
        case 'ArrowLeft': this.action = Tetrimino.ACTION_MOVE_LEFT; break;
        case 'ArrowDown': this.action = Tetrimino.ACTION_MOVE_DOWN; break;
        case 'Space': this.action = Tetrimino.ACTION_ROTATE; break;
    }
},

    setGrid: function(tetrisGrid) {
    this.grid = tetrisGrid;
},

    getPaddings: function() {
    return Tetrimino.getPaddings(this.bricksMap);
}

  });
Tetrimino.ACTION_NONE = 0;
Tetrimino.ACTION_MOVE_LEFT = 1;
Tetrimino.ACTION_MOVE_RIGHT = 2;
Tetrimino.ACTION_MOVE_DOWN = 3;
Tetrimino.ACTION_STOP_MOVE = 4;
Tetrimino.ACTION_ROTATE = 5;
Tetrimino.SIZE = 4;
Tetrimino.getPaddings = function(bricksMap) {
    var paddings = { top: 0, right: 0, left: 0};
var ri = Tetrimino.SIZE;
while (ri--)
{
    if (!Grid.rowIsEmpty(bricksMap[ri])) break;
    paddings.top++;
}
for (var ci = 0; ci < Tetrimino.SIZE; ci++)
{
    if (!Grid.colIsEmpty(bricksMap, ci)) break;
    paddings.left++;
}
var ci = Tetrimino.SIZE;
while (ci--)
{
    if (!Grid.colIsEmpty(bricksMap, ci)) break;
    paddings.right++;
}
return paddings;
  };
Tetrimino.PLANS = [

    [
        // square
      [

        [0, 0, 0, 0],
          [0, 1, 1, 0],
          [0, 1, 1, 0],
          [0, 0, 0, 0]
        ]
      ],
      // T
      [
        [
          [0, 0, 0, 0],
          [0, 1, 0, 0],
          [1, 1, 1, 0],
          [0, 0, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 1, 0],
          [0, 1, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 0, 0, 0],
          [1, 1, 1, 0],
          [0, 1, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 0, 1, 0],
          [0, 1, 1, 0],
          [0, 0, 1, 0]
        ]
      ],
       //I
      [
        [
          [0, 1, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 0, 0, 0],
          [1, 1, 1, 1],
          [0, 0, 0, 0]
        ]
      ],
      // L
      [
        [
          [0, 0, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 1, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 1, 1],
          [0, 1, 0, 0],
          [0, 0, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 1, 0],
          [0, 0, 1, 0],
          [0, 0, 1, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 0, 1, 0],
          [1, 1, 1, 0],
          [0, 0, 0, 0]
        ]
      ],

      // L - reversed
      [
        [
          [0, 0, 0, 0],
          [0, 0, 1, 0],
          [0, 0, 1, 0],
          [0, 1, 1, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 1, 1],
          [0, 0, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 1, 0],
          [0, 1, 0, 0],
          [0, 1, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [1, 1, 1, 0],
          [0, 0, 1, 0],
          [0, 0, 0, 0]
        ]
      ],


      // Z
      [
        [
          [0, 0, 0, 0],
          [0, 0, 1, 0],
          [0, 1, 1, 0],
          [0, 1, 0, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 1, 1, 0],
          [0, 0, 1, 1],
          [0, 0, 0, 0]
        ]
      ],

      // Z - reversed
      [
        [
          [0, 0, 0, 0],
          [0, 1, 0, 0],
          [0, 1, 1, 0],
          [0, 0, 1, 0]
        ],
        [
          [0, 0, 0, 0],
          [0, 0, 1, 1],
          [0, 1, 1, 0],
          [0, 0, 0, 0]
        ]
      ]

  ];


}
}
