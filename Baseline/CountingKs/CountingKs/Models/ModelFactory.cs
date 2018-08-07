using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace CountingKs.Models
{
    public class ModelFactory
    {
        private ICountingKsRepository _repo;
        private UrlHelper _urlHelper;

        public ModelFactory(HttpRequestMessage request, ICountingKsRepository repo)
        {
            _urlHelper = new UrlHelper(request);
            _repo = repo;
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
                Links = new List<LinkModel>
                {
                    CreateLink(_urlHelper.Link("Diaries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }), "self"),
                    CreateLink(_urlHelper.Link("DiaryEntries", new { diaryid = diary.CurrentDate.ToString("yyyy-MM-dd") }), "newDiaryEntry", "POST")
                },
                // of what user should type in browser to hit the corrsponding function. So we needed to convert into this format now by seeing the URI in output user knows
                // in what format he should be making subsequent request.
                CurrentDate = diary.CurrentDate,
                Entries = diary.Entries.Select(e => Create(e))
            };
        }

        public LinkModel CreateLink(string href, string rel, string method = "GET", bool isTemplated = false)
        {
            return new LinkModel
            {
                Href = href,
                Rel = rel,
                IsTemplated = isTemplated,
                Method = method
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

        public DiaryEntry Parse(DiaryEntryModel model)
        {
            try
            {
                var entry = new DiaryEntry();
                if (model.Quantity != default(double))
                {
                    entry.Quantity = model.Quantity;
                }
                if (!string.IsNullOrEmpty(model.MeasureUrl))
                {
                    var uri = new Uri(model.MeasureUrl);
                    var measureId = int.Parse(uri.Segments.Last());
                    var measure = _repo.GetMeasure(measureId);
                    entry.Measure = measure;
                    entry.FoodItem = measure.Food;
                }
                return entry;
            }
            catch
            {
                return null;
            }
        }
        public Diary Parse(DiaryModel model)
        {
            try
            {
                var entity = new Diary();
                var selfLink = model.Links.Where(l => l.Rel == "self").FirstOrDefault();
                if (selfLink!= null && !string.IsNullOrWhiteSpace(selfLink.Href))
                {
                    var uri = new Uri(selfLink.Href);
                    entity.Id = int.Parse(uri.Segments.Last());
                }

                entity.CurrentDate = model.CurrentDate;

                if (model.Entries != null)
                {
                    foreach (var entry in model.Entries) entity.Entries.Add(Parse(entry));
                }

                return entity;
            }
            catch
            {
                return null;
            }
        }


        public DiarySummaryModel CreateSummary(Diary diary)
        {
            return new DiarySummaryModel()
            {
                DiaryDate = diary.CurrentDate,
                TotalCalories = Math.Round(diary.Entries.Sum(e => e.Quantity * e.Measure.Calories))
            };
        }

        public MeasureV2Model CreateV2(Measure measure)
        {
            return new MeasureV2Model
            {
                Url = _urlHelper.Link("Measures", new { foodid = measure.Food.Id, id = measure.Id}), // this Measures2 name needs to be mapped in route.config file route name.
                Description = measure.Description,
                Calories = measure.Calories,
                Carbohydrates = measure.Carbohydrates,
                Cholestrol = measure.Cholestrol,
                Fiber = measure.Fiber,
                Iron = measure.Iron,
                Protein = measure.Protein,
                SaturatedFat = measure.SaturatedFat,
                Sodium = measure.Sodium,
                Sugar = measure.Sugar,
                TotalFat = measure.TotalFat
            };
        }
    }
}
// Problem here is that in serialization, even if they are not needed we are showing them
// I mean suppose if by default all the rel is "GET" then out Response will be populated with "Get" and also if isTemplated is true then only we want to show it
// but out serialization that is provided by default does not understand all this and will show everything in each response