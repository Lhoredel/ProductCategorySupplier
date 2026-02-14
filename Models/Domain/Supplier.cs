namespace ProductCategorySupplier.Models.Domains
{
    public class Supplier
    {
        public Guid SupplierId { get; set;}
        public string SupplierName { get; set;}
       public string  Address { get; set; }
       public string Email { get; set; }
       public string TaxCode { get; set; }
    }
}
