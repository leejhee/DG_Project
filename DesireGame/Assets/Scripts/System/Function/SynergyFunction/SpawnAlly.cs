using UnityEngine;

namespace Client
{
    public class SpawnAlly : FunctionBase
    {
        private CharBase _spawnedAlly;
        
        public SpawnAlly(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool startExecution = true)
        {
            if (startExecution)
            {
                int spawnIndex = TileManager.Instance.GetSmallestAllyTileIndex();
                if (spawnIndex == -1)
                {
                    Debug.Log("자리가 꽉차서 소환수를 소환할 수 없습니다. 비워주세요.");
                    //TODO : 소환수가 소환될 수 있을 때까지 기다렸다 자리가 날 때 소환될 수 있게 해야함.
                    return;
                }

                _spawnedAlly = CharManager.Instance.CharGenerate(new CharTileParameter
                (
                    scene: SystemEnum.eScene.GameScene,
                    tileIndex: spawnIndex,
                    index: _FunctionData.input1)
                );
            }
            else
            {
                if (_spawnedAlly)
                {
                    _spawnedAlly.Dead();
                }
            }
        }
    }
}