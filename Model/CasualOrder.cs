using System.Collections.Generic;

namespace WSOrderCreator.Model
{
  public class CasualOrder
  {
    public int EntryID { get; set; }

    public int TotalOrderPrice { get; set; }

    public string AffiliateName { get; set; }

    public int ShopID { get; set; }

    public ShopperDetails ShopperDetails { get; set; }

    public string DateCreated { get; set; }

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();

    public ShippingDetails ShippingDetails { get; set; }
  }
}
