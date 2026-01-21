using Data.Progression;

namespace Databases
{
    public class UpgradeDatabase : DatabaseBase<UpgradeData>
    {
        protected override string GetKey(UpgradeData entry) => entry.ID;
    }
}