using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaMap.AncientArks;
namespace SagaMap.Tasks.AncientArk
{
    public class AncientArkQuest : MultiRunTask
    {
        SagaMap.AncientArks.AncientArk AA;
        Map map;
        public int lifeTime;
        public int counter = 0;
        public AncientArkQuest(Map map, int lifeTime)
        {
            this.period = 1000;
            this.dueTime = 1000;
            this.map = map;
            this.lifeTime = lifeTime;
        }

        public override void CallBack()
        {
            try
            {
                map.AncientArk.Gimmick_Count--;

                if (map.AncientArk.Gimmick_Count <= 0)
                {
                    this.Deactivate();
                    map.AncientArk.QuestTask = null;
                    if(map.AncientArk.Gimmick_ID_layer != 0)
                    {
                        map.AncientArk.complete_layer = true;
                        map.AncientArk.Gimmick_ID_layer = 0;
                    }
                    SagaMap.Manager.AncientArkManager.Instance.完成任務_更新任務or下一層(map);
                    return;
                }
                if (map.AncientArk.Gimmick_Count == 250 ||                    
                    map.AncientArk.Gimmick_Count == 200 ||
                    map.AncientArk.Gimmick_Count == 150 ||
                    map.AncientArk.Gimmick_Count == 100 ||
                    map.AncientArk.Gimmick_Count == 50 ||
                    map.AncientArk.Gimmick_Count == 30 ||
                    map.AncientArk.Gimmick_Count == 10 ||
                    map.AncientArk.Gimmick_Count == 5)
                {
                    map.Announce("剩餘時間：" + map.AncientArk.Gimmick_Count + "秒");
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }            
        }
    }
}
