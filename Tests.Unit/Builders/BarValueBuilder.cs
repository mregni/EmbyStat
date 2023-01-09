using EmbyStat.Common.Models.Charts;

namespace Tests.Unit.Builders;

public class BarValueBuilder<T1, T2>
{
    private BarValue<T1,T2> _barValue;

    public BarValueBuilder()
    {
        _barValue = new BarValue<T1, T2>();
    }
    public BarValue<T1, T2> Build()
    {
        return _barValue;
    }
}