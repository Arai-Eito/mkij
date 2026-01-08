using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;
using static UnityEngine.Rendering.DebugUI;

public class Stage : MonoBehaviour
{
    static public Stage instance;

    [SerializeField] Transform _base;

    [SerializeField] int _puzzleArea = 4;
    [SerializeField] Taiho _taiho;


    [SerializeField] GameObject _blocknone;
    [SerializeField] GameObject _enemy;
    [SerializeField] GameObject _blocknormal;
    [SerializeField] GameObject _present;


    int3 _stageSize;
    Vector3 _basePoint;

    Block[,] _blocks;

    int _level = 0;
    int _killPosition = 0;
    int _killPositionPlusBorder = 7;
    int _killPositionMinusBorder = 4;
    int _presentCount = 0;
    [SerializeField] private float stepTime = 5f;
    private float currentTime;



    [SerializeField] GameObject _clearLineEffect;
    [SerializeField] GameObject _deleteEffect;

    [Header("消したときの光るオーブ")]
    [SerializeField] private LineClearOrbAnimator _lineClearOrbAnimator;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        Vector3 stageSize = _base.localScale;
        _stageSize.x = (int)stageSize.x;
        _stageSize.y = (int)stageSize.y;
        _stageSize.z = (int)stageSize.z;

        // 
        _basePoint = _base.position - stageSize * 0.5f + new Vector3(0.5f, stageSize.y + 0.1f, 0.5f);

        {
            _blocks = new Block[_stageSize.z, _stageSize.x];

            for (int z = 0; z < _stageSize.z; z++)
            {
                for (int x = 0; x < _stageSize.x; x++)
                {
                    Vector3 spawnposition = _basePoint + new Vector3(x, 0, z);

                    GameObject obj = Instantiate(
                        _blocknone, _basePoint + new Vector3(0, -1, -10),
                        Quaternion.identity);
                    obj.transform.SetParent(_base);

                    Block spawnBlock = obj.GetComponent<Block>();
                    spawnBlock.MoveTo(spawnposition);
                    spawnBlock.SetIndex(new int2(x,z));

                    _blocks[z, x] = spawnBlock;
                }
            }
        }

        for(int i = 0; i < 8;i++)
        {
            NextTurn();
        }
    }

    ////////////////////////////////////////////////////////
    /// TURN
    /// 
    public void NextTurn()
    {
        int lncrease = (int)(_killPosition * 0.2f) ;
        _level+= 1 + lncrease;
        Debug.Log("level "+_level+"   lncrease "+ (1 + lncrease) + "   killposition "+_killPosition );
        Move();


        // プレゼント
        if(_presentCount <= 0)
        {
            _presentCount = UnityEngine.Random.Range(4, 7);
            SpawnPresent(_level, 1);
        }
        _presentCount--;
        // 5% を 5かい抽選
        for (int i = 0; i < 5; i ++)
        {
            if( UnityEngine.Random.Range(0, 33) == 0)
            {
                SpawnPresent(_level, 1);
            }
        }

        // 敵をスポーン
        int num = UnityEngine.Random.Range(0, _stageSize.x);
        SpawnEnemy(_level, num);

        // 体力０
        for (int z = 0; z < _stageSize.z; z++)
        {
            for (int x = 0; x < _stageSize.x; x++)
            {
                Block block = _blocks[z, x];
                if (block.GetBlockType() == BLOCK_TYPE.NONE) continue;


                if (block.GetNumber() <= 0)
                {
                    DeleteBlock(new int2(x, z));
                }
            }
        }

    }
    public void CheckLineClear()
    {
        for (int z = 0; z < _puzzleArea; z++)
        {
            bool clear = true;
            int totalNumber = 0;

            for (int x = 0; x < _stageSize.x; x++)
            {
                Block block = _blocks[z, x];

                // clear しない
                if (block.GetBlockType() != BLOCK_TYPE.NOMOVE)
                {
                    clear = false;
                    break;
                }


                totalNumber += block.GetNumber();
            }

            if (!clear) continue;

            // 各ブロックの「位置＋値」を集める（消す前）
            var items = new List<LineClearOrbAnimator.OrbItem>(_stageSize.x);

            for (int x = 0; x < _stageSize.x; x++)
            {
                Block block = GetBlock(new int2(x, z));
                Vector3 position = block.GetPosition();

                // オーブ用に登録
                items.Add(new LineClearOrbAnimator.OrbItem(position, block.GetNumber()));
                
                GameObject fx = Instantiate(_clearLineEffect, position + Vector3.up * 1.0f, transform.rotation);
                fx.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


                DeleteBlock(new int2(x, z));
            }

            // 合計を大砲に加算
            if (_lineClearOrbAnimator != null)
            {
                // 到着するたびに加算
                _lineClearOrbAnimator.Play(
                    items,
                    _taiho.transform,
                    (v) =>
                    {
                        _taiho.SetNumber(_taiho.GetNumber() + v);
                        _taiho.UpdateNumberText();
                    }
                );
            }
            else
            {
                // 演出無しなら即時合計加算
                
                _taiho.SetNumber(_taiho.GetNumber() + totalNumber);
                _taiho.UpdateNumberText();
            }
        }
    }

    ////////////////////////////////////////////////////////
    /// 

    void Move()
    {
        for (int z = 0; z < _stageSize.z; z++)
        {
            for (int x = 0; x < _stageSize.x; x++)
            {
                if (_blocks[z, x].GetBlockType() != BLOCK_TYPE.MOVE) continue;


                _blocks[z, x].Move();
            }

        }
    }

    /// ////////////////////////////////////////////////////
    /// SPAWN
    void SpawnEnemy(int level, int num)
    {
        if (num == 0 || level == 0) return;

        if (_stageSize.x < num) num = _stageSize.x;


        for (int i = 0; i < num; i++)
        {
            for (int n = 0; n < 100; n++)
            {
                int randomX = UnityEngine.Random.Range(0, _stageSize.x);

                // 行先確認
                Block block = _blocks[_stageSize.z - 1, randomX];
                if (block.GetBlockType() != BLOCK_TYPE.NONE) continue;

                // スポーン
                Vector3 position = _basePoint + new Vector3(randomX, 0.0f, _stageSize.z - 1);

                GameObject obj = Instantiate(_enemy, position, quaternion.identity);
                Block spawnBlock = obj.GetComponent<Block>();
                spawnBlock.SetNumber(level + UnityEngine.Random.Range(0, 10));
                spawnBlock.MoveTo(position);
                spawnBlock.SetIndex(new int2(randomX,_stageSize.z - 1));

                // 設置
                Destroy(_blocks[_stageSize.z - 1, randomX].gameObject);
                _blocks[_stageSize.z - 1, randomX] = spawnBlock;


                break;
            }
        }
    }

    void SpawnPresent(int level, int num)
    {
        if (num == 0 || level == 0) return;

        if (_stageSize.x < num) num = _stageSize.x;


        for (int i = 0; i < num; i++)
        {
            for (int n = 0; n < 100; n++)
            {
                int randomX = UnityEngine.Random.Range(0, _stageSize.x);

                // 行先確認
                Block block = _blocks[_stageSize.z - 1, randomX];
                if (block.GetBlockType() != BLOCK_TYPE.NONE) continue;

                // スポーン
                Vector3 position = _basePoint + new Vector3(randomX, 0.0f, _stageSize.z - 1);

                GameObject obj = Instantiate(_present, position, quaternion.identity);
                Block spawnBlock = obj.GetComponent<Block>();
                spawnBlock.SetNumber(level + UnityEngine.Random.Range(0, 10));
                spawnBlock.MoveTo(position);
                spawnBlock.SetIndex(new int2(randomX,_stageSize.z - 1));

                // 設置
                Destroy(_blocks[_stageSize.z - 1, randomX].gameObject);
                _blocks[_stageSize.z - 1, randomX] = spawnBlock;


                break;
            }
        }
    }

    ////////////////////////////////////////////////////////
    ///
    public int2 GetIndex(Vector3 worldPosition)
    {
        Vector3 direction =
            worldPosition +
            new Vector3(0.5f, 0.0f, 0.5f) -
            _basePoint;


        int2 index = new int2(
            Mathf.FloorToInt(direction.x),
            Mathf.FloorToInt(direction.z));


        return index;
    }
    public Block GetBlock(int2 index) { return _blocks[index.y, index.x]; }
    public bool CheckIndex(int2 index)
    {
        if (index.x < 0 || _stageSize.x <= index.x ||
            index.y < 0 || _stageSize.z <= index.y) return false;

        return true;
    }
    public bool CheckSetBlock(int2 index)
    {
        if (CheckIndex(index) == false) return false;
        if (index.y >= _puzzleArea) return false;
        if (_blocks[index.y, index.x].GetBlockType() != BLOCK_TYPE.NONE) return false;

        return true;
    }
    public void SetBlock(int2 index, Block block)
    {
        Block b = _blocks[index.y, index.x];

        _blocks[index.y, index.x] = block;
        block.MoveTo(b.GetPosition());
        block.transform.SetParent(_base);

        Destroy(b.gameObject);
    }
    public void DeleteBlock(int2 index)
    {
        if (CheckIndex(index))
        {
            Block block = _blocks[index.y, index.x];
            block.Broken();

            // killpositon 
            if(block.GetBlockType() == BLOCK_TYPE.MOVE)
            {
                if(_killPositionPlusBorder <= index.y)
                {
                    _killPosition++;
                }
                else if(index.y <= _killPositionMinusBorder)
                {
                    _killPosition--;
                }
            }


            Vector3 position = block.GetPosition();
            Destroy(block.gameObject);


            // あたらしいブロックを用意する
            GameObject obj = Instantiate(_blocknone, position, Quaternion.identity);
            block = obj.GetComponent<Block>();
            block.MoveTo(position);
            block.SetIndex(index);
            _blocks[index.y, index.x] = block;
        }
    }
    public void SwapBlock(int2 from, int2 to)
    {
        if (!CheckIndex(from) || !CheckIndex(to)) return;

        Block fromblock = _blocks[from.y,from.x];
        Block toblock = _blocks[to.y,to.x];

        _blocks[to.y, to.x] = fromblock;
        _blocks[from.y,from.x] = toblock;

        Vector3 fromblock_ToPosition = fromblock.GetPosition();
        Vector3 toblock_ToPosition = toblock.GetPosition();

        fromblock.MoveTo(toblock_ToPosition);
        toblock.MoveTo(fromblock_ToPosition);

        int2 fromblock_index = fromblock.GetIndex();
        int2 toblock_index = toblock.GetIndex();

        fromblock.SetIndex(toblock_index);
        toblock.SetIndex(fromblock_index);

    }
    public Taiho GetTaiho() { return _taiho; }

    public int GetLevel() { return _level; }
    ////////////////////////////////////////////////////////
    /// UI BUTTON 等
 
    
}
