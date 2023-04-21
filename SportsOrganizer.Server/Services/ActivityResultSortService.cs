using SportsOrganizer.Data.Enums;
using SportsOrganizer.Server.Models;

namespace SportsOrganizer.Server.Services;

public class ActivityResultSortService
{
    public static List<ActivityResultSortModel> SortActivityResults(List<ActivityResultSortModel> models, OrderType orderType)
    {
        int place = 1;

        if (orderType == OrderType.Ascending)
        {
            models = models.OrderBy(x => x.ActivityResult?.Result ?? Double.PositiveInfinity).ToList();

            var previousModel = new ActivityResultSortModel();

            for (int i = 0; i < models.Count(); i++)
            {
                if (previousModel.Place != null)
                {
                    if (models[i].ActivityResult != null)
                    {
                        if (models[i].ActivityResult.Result > previousModel.ActivityResult.Result)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                    else
                    {
                        if (previousModel.ActivityResult != null)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                }
                else
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
            }
        }
        else
        {
            models = models.OrderByDescending(x => x.ActivityResult?.Result ?? Double.NegativeInfinity).ToList();

            var previousModel = new ActivityResultSortModel();

            for (int i = 0; i < models.Count(); i++)
            {
                if (previousModel.Place != null)
                {
                    if (models[i].ActivityResult != null)
                    {
                        if (models[i].ActivityResult.Result < previousModel.ActivityResult.Result)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                    else
                    {
                        if (previousModel.ActivityResult != null)
                        {
                            models[i].Place = place;
                            previousModel = models[i];
                            place++;
                        }
                        else
                        {
                            models[i].Place = previousModel.Place;
                            place++;
                        }
                    }
                }
                else
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
            }
        }

        return models;
    }

    public static List<ActivityResultScoresModel> SortActivityResultScores(List<ActivityResultScoresModel> models)
    {
        int place = 1;

        models = models.OrderBy(x => x.Points).ToList();

        var previousModel = new ActivityResultScoresModel();

        for (int i = 0; i < models.Count(); i++)
        {
            if (previousModel.Place != 0)
            {
                if (models[i].Points > previousModel.Points)
                {
                    models[i].Place = place;
                    previousModel = models[i];
                    place++;
                }
                else
                {
                    models[i].Place = previousModel.Place;
                    place++;
                }
            }
            else
            {
                models[i].Place = place;
                previousModel = models[i];
                place++;
            }
        }

        return models;
    }
}
