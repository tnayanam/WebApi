using CountingKs.Data.Entities;
using System.Linq;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        public FoodModel Create(Food food)
        {
            return new FoodModel
            {
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel
            {
                Description = measure.Description,
                Calories = measure.Calories
            };
        }
    }
}