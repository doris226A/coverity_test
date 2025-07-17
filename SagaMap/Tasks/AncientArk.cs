using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using SagaLib;
using SagaMap.AncientArks;
namespace SagaMap.Tasks.AncientArk
{
    public class AncientArk : MultiRunTask
    {
        SagaMap.AncientArks.AncientArk AA;
        public int lifeTime;
        public int counter = 0;
        public AncientArk(SagaMap.AncientArks.AncientArk AncientArk, int lifeTime)
        {
            this.period = 1000;
            this.dueTime = 1000;
            this.AA = AncientArk;
            this.lifeTime = lifeTime;
        }

        public override void CallBack()
        {
            try
            {
                this.AA.Time--;
                counter++;
                int rest = lifeTime - counter;

                if (rest == 0)
                {
                    this.Deactivate();
                    this.AA.Destory(DestroyType.TimeOver);
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.ShowError(ex);
            }            
        }
    }
}
