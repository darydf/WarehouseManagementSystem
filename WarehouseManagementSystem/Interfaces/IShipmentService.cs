using System.Data;
using System.Collections.Generic;
using WarehouseManagementSystem.Models;

namespace WarehouseManagementSystem.Services
{
    public interface IShipmentService
    {
        Shipment CreateDraft();
        string GenerateShipmentNumber();
        bool CheckStockAvailability(int productId, decimal quantity);
        decimal GetAvailableStock(int productId);
        bool ProcessShipment(Shipment shipment);
        DataTable GetAllShipments();
        DataTable GetShipmentsByStorekeeper(int storekeeperId);
        DataTable GetShipmentDetails(int shipmentId);
    }
}
