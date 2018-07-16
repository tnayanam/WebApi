using System.Collections.Generic;
// this is more like a view model
namespace CountingKs.Models
{
    public class FoodModel
    {
        public string Description { get; set; }
        public IEnumerable<MeasureModel> Measures { get; set; }
    }
}