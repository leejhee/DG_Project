namespace Client
{
    public class ApplyCC : FunctionBase
    {
        private CCBase CC = null;

        public ApplyCC(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (StartFunction)
            {
                CC = CCFactory.CCGenerate(new CCParameter()
                {
                    Caster = _CastChar,
                    Target = _TargetChar,
                    ccType = _FunctionData.CCType
                });
                CC?.RunCC();
            }
            else
            {
                CC?.EndCC();
            }

        }

    }
    
}
