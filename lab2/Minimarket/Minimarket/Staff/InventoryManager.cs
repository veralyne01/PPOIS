using System.Linq;

namespace Minimarket
{
    public class InventoryManager : Employee
    {
        public InventoryManager(string name)
            : base(name)
        {
        }

        public InventoryReport PerformInventoryCheck(
            Warehouse warehouse,
            IReadOnlyList<Department> departments,
            DateOnly date)
        {
            if (warehouse == null)
                throw new ArgumentNullException(nameof(warehouse));
            if (departments == null)
                throw new ArgumentNullException(nameof(departments));

            InventoryCheck check = new(date, this);

            foreach (ProductBatch batch in warehouse.Batches)
            {
                check.AddItem(new InventoryCheckItem(
                    batch.product,
                    batch.quantity,
                    batch.quantity,
                    Location.Warehouse,
                    batch.expirationDate
                ));
            }

            foreach (Department department in departments)
            {
                foreach (Shelf shelf in department.Shelves)
                {
                    foreach (ProductBatch batch in shelf.Batches)
                    {
                        check.AddItem(new InventoryCheckItem(
                            batch.product,
                            batch.quantity,
                            batch.quantity,
                            new Location(department, shelf),
                            batch.expirationDate
                        ));
                    }
                }
            }

            check.Complete();
            return new InventoryReport(check);
        }
    }
}
