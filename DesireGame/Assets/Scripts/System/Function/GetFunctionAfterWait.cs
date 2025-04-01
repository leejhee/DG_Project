namespace Client
{
    public class GetFunctionAfterWait : FunctionBase
    {

        public GetFunctionAfterWait(BuffParameter buffParam) : base(buffParam)
        {
        }

        public override void RunFunction(bool StartFunction = true)
        {
            base.RunFunction(StartFunction);
            if (!StartFunction)
            {
                var info = DataManager.Instance.GetData<FunctionData>(_FunctionData.input1);
                _TargetChar.FunctionInfo.AddFunction(new BuffParameter()
                {
                    CastChar = _CastChar,
                    TargetChar = _TargetChar,
                    FunctionIndex = info.Index,
                    eFunctionType = info.function
                });

            }
        }
    }
}