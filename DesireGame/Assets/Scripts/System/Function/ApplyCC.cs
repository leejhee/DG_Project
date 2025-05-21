namespace Client
{
    public class ApplyCC : FunctionBase
    {
        private NegativeEffectBase _negativeEffect = null;

        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _negativeEffect = CCFactory.CCGenerate(new CCParameter()
                {
                    Caster = _CastChar,
                    Target = _TargetChar,
                    ccType = _FunctionData.CCType
                });
                _negativeEffect?.RunEffect();
            }
            else
            {
                _negativeEffect?.EndEffect();
            }

        }

    }
    
}
