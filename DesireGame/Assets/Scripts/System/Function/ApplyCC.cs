namespace Client
{
    public class ApplyCC : FunctionBase
    {
        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        { }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                _TargetChar.EffectInfo.AddEffect(new EffectParameter()
                {
                    Caster = _CastChar,
                    Target = _TargetChar,
                    ccType = _FunctionData.CCType,
                    Time = _FunctionData.time,
                });
            }
        }

    }
    
}
