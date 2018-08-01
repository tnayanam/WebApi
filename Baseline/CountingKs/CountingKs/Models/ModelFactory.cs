﻿using CountingKs.Data;
using CountingKs.Data.Entities;
using System;
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

                if (!string.IsNullOrWhiteSpace(model.Url))
                {
                    var uri = new Uri(model.Url);
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