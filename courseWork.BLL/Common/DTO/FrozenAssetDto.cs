namespace courseWork.BLL.Common.DTO
{
    public class FrozenAssetDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public int FrozenAssetsQty { get; set; }
    }
}

