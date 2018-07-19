using CountingKs.Data.Entities;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request)
        {
            _urlHelper = new UrlHelper(request);
        }
        public FoodModel Create(Food food)
        {
            return new FoodModel
            {
                Url = _urlHelper.Link("Food", new { foodid = food.Id }),
                Description = food.Description,
                Measures = food.Measures.Select(m => Create(m))
            };
        }

        public MeasureModel Create(Measure measure)
        {
            return new MeasureModel
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id }),
                Description = measure.Description,
                Calories = measure.Calories
            };
        }

        public DiaryModel Create(Diary diary)
        {
            return new DiaryModel
            {
                Url = _urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }), // no matter how the date is stored in DB we want to show te url in such a way
                // of what user should type in browser to hit the corrsponding function. So we needed to convert into this format now by seeing the URI in output user knows
                // in what format he should be making subsequent request.
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(e => Create(e))
            };
        }
        public DiaryEntryModel Create(DiaryEntry entry)
        {
            return new DiaryEntryModel
            {
                Url = _urlHelper.Link("DiaryEntries", new { diaryid = entry.Diary.CurrentDate.ToString("yyyy-MM-dd"), id = entry.Id }),
                Quantity = entry.Quantity,
                FoodDescription = entry.FoodItem.Description,
                MeasureDescription = entry.Measure.Description,
                MeasureUrl = _urlHelper.Link("Measures", new { foodid = entry.FoodItem.Id, id = entry.Measure.Id })
            };
        }
    }
}