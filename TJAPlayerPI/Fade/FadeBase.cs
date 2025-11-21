using FDK;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TJAPlayerPI.Fade
{
    internal abstract class FadeBase : CActivity
    {
        public abstract float DefaultFadeOutInterval { get; }
        public abstract float DefaultFadeInInterval { get; }

        public FadeState State { get; private set; }
        public float Value { get; private set; }
        public bool Ended => counter?.n現在の値 == counter?.n終了値;

        public FadeBase()
        {

        }

        public override void On活性化()
        {
            if (this.b活性化してる)
                return;




            base.On活性化();
        }

        public override void On非活性化()
        {
            if (this.b活性化してない)
                return;

            base.On非活性化();
        }

        public virtual int OnUpdate()
        {
            if (this.b活性化してない)
                return 0;

            if (counter is null || State == FadeState.None)
                return 0;

            counter.t進行();

            Value = counter.n現在の値 / (float)counter.n終了値;

            if (counter.n現在の値 == counter.n終了値)
            {
                finish();
            }

            return 0;
        }

        public override int On進行描画()
        {
            if (this.b活性化してない)
                return 0;


            return 0;
        }

        public virtual void StartFadeOut(float interval, Action? finished = null)
        {
            counter = new CCounter(0, (int)(interval * 1000), 1, TJAPlayerPI.app.Timer);
            this.finished = finished;
            Value = 0.0f;

            State = FadeState.FadeOut;
        }

        public virtual void StartFadeIn(float interval, Action? finished = null)
        {
            counter = new CCounter(0, (int)(interval * 1000), 1, TJAPlayerPI.app.Timer);
            this.finished = finished;
            Value = 0.0f;

            State = FadeState.FadeIn;
        }

        private CCounter? counter;
        private Action? finished;

        private void finish()
        {
            counter = null;

            switch (State)
            {
                case FadeState.FadeOut:
                    {
                        State = FadeState.Wait;
                        Value = 1.0f;

                        finished?.Invoke();
                        finished = null;
                    }
                    break;
                case FadeState.FadeIn:
                    {
                        State = FadeState.None;
                        Value = 0.0f;

                        finished?.Invoke();
                        finished = null;
                    }
                    break;
            }
        }
    }
}
