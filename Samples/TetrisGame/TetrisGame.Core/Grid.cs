using System;
using System.Collections.Generic;
using System.Text;

namespace TetrisGame.Core
{
    var Grid = ctx.Grid = cc.Node.extend({

    ctor: function() {
      this._super();
      this.size = new cc.Rect(0, 0, 10, 20);
      this.setContentSize(this.size.width* BrickSprite.WIDTH, this.size.height* BrickSprite.HEIGHT);
      this.tetrimino = null;
      this.bricksMap = [];
      this.level = 0;
      this.setLevel(0);
    },

    update: function(dt)
    {
        if (!this.tetrimino) return;
        this.tetrimino.update(dt);
        if (this.tetrimino.isFrozen) delete this.tetrimino;
    },

    pushTetrimino: function(tetrimino)
    {
        tetrimino.setGrid(this);
        this.addChild(tetrimino);
        this.tetrimino = tetrimino;
    },

    setLevel: function(level)
    {
        this.level = level;
        this.bricksMap = Grid.createBricksMap(this.size.width, this.size.height, level * 2);
        this.tetrimino = null;
        this._updateBriks();
    },

    addBricksFromTetrimino: function(tetrimino)
    {
        var ri = Tetrimino.SIZE;
        while (ri--)
        {
            for (var ci = 0; ci < Tetrimino.SIZE; ci++)
            {
                if (!tetrimino.bricksMap[ri][ci]) continue;
                var brickPos = { x: tetrimino.gridPos.x + ci, y: tetrimino.gridPos.y + (Tetrimino.SIZE - ri - 1)};
            this.bricksMap[brickPos.y][brickPos.x] = 1;
        }
    }
    var gameOver = !Grid.rowIsEmpty(this.bricksMap[this.size.height - 1]);
      if (gameOver) {
        cc.game.state.gameOver();
      }
this._updateBriks();
    },

    _updateBriks: function() {
    this.removeAllChildren();

    // remove completed rows
    var removedCount = 0;
    this.bricksMap = this.bricksMap.filter(function(row) {
        if (!Grid.rowIsCompleted(row)) return true;
        removedCount++;
        return false;
    });
    if (removedCount)
    {
        $clearSound.play();
        cc.game.state.addPointsForRowsCount(removedCount);
    }
    while (removedCount--)
    {
        this.bricksMap.push(Grid.createRow(this.size.width));
    }

    // create bricks sprites
    for (var ri = 0; ri < this.size.height; ri++)
    {
        for (var ci = 0; ci < this.size.width; ci++)
        {
            if (!this.bricksMap[ri][ci]) continue;
            var brick = new BrickSprite();
            brick.setPosition(
                ci * BrickSprite.WIDTH + BrickSprite.WIDTH / 2,
                ri * BrickSprite.HEIGHT
            );
            this.addChild(brick);
        }
    }
}

  });

Grid.createBricksMap = function(width, height, level) {
    var bricksMap = [];
    for (var ri = 0; ri < height; ri++)
    {
        var rowHasBricks = ri < level;
        bricksMap.push(Grid.createRow(width, rowHasBricks));
    }
    return bricksMap;
};

Grid.rowIsCompleted = function(row) {
    var ci = row.length;
    while (ci--)
    {
        if (!row[ci]) return false;
    }
    return true;
};

Grid.rowIsEmpty = function(row) {
    var ci = row.length;
    while (ci--)
    {
        if (row[ci]) return false;
    }
    return true;
};

Grid.colIsEmpty = function(bricksMap, colInd) {
    var ri = bricksMap.length;
    while (ri--)
    {
        if (bricksMap[ri][colInd]) return false;
    }
    return true;
};

Grid.createRow = function(width, needCreateBricks) {
    var row = [];
    var ci = width;
    while (ci--)
    {
        var hasBrick = needCreateBricks ? Math.round(Math.random()) : 0;
        row.push(hasBrick);
    }
    return row;
};

}
}
