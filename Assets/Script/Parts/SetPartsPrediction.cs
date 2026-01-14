using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class SetPartsPrediction : MonoBehaviour
{
    static public SetPartsPrediction instance;

    [SerializeField] Material _successMaterial;
    [SerializeField] Material _failMaterial;

    [SerializeField] GameObject _prefabPredictionBlock;


    private const int _blockNum = 5;

    //////////////////////////////////////

    struct PredictionBlockInfo
    {
        public Transform _transform;
        public MeshRenderer _renderer;
    }
    private PredictionBlockInfo[] _predictionBlocks = new PredictionBlockInfo[_blockNum];


    //////////////////////////////////////

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
        for(int i = 0; i < _blockNum; i ++)
        {
            GameObject obj = Instantiate(_prefabPredictionBlock);

            _predictionBlocks[i]._transform = obj.transform;
            _predictionBlocks[i]._renderer = obj.GetComponent<MeshRenderer>();
            _predictionBlocks[i]._renderer.enabled = false;
        }
    }

    //////////////////////////////////////
    public void InvisiblePredictionBlock(int index)
    {
        if (index < 0 || _blockNum <= index) return;

        _predictionBlocks[index]._renderer.enabled = false;
    }
    public void SetPredictionBlock(int index,Vector3 position,bool success)
    {
        if (index < 0 || _blockNum <= index) return;

        _predictionBlocks[index]._transform.position = position;
        _predictionBlocks[index]._renderer.enabled = true;

        Material[] mats = _predictionBlocks[index]._renderer.materials;
        mats[0] = (success) ? _successMaterial : _failMaterial;
        _predictionBlocks[index]._renderer.materials = mats;

    }
}
