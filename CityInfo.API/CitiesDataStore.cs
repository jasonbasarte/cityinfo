using CityInfo.API.Models;

namespace CityInfo.API
{
    public class CitiesDataStore
    {
        public List<CityDto> Cities { get; set; }

        //public static CitiesDataStore Current { get; set; } = new CitiesDataStore();
        public CitiesDataStore() {
            Cities = new List<CityDto>()
            {
                new CityDto() 
                {
                    Id = 1,
                    Name = "New York City",
                    Description = "The one with that big park",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterestDto
                        {
                            Id = 2,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Antwerp",
                    Description = "The one with the cathedral that was never really fit",
                       PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto
                        {
                            Id = 1,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterestDto
                        {
                            Id = 2,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                             new PointOfInterestDto
                        {
                            Id = 3,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterestDto
                        {
                            Id = 4,
                            Name = "Central Park",
                            Description = "The most visited urban park in the United States."
                        }
                    }
                },
                new CityDto()
                {
                    Id = 3,
                    Name = "Paris",
                    Description = "The one with that big tower",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto
                        {
                            Id = 1,
                            Name = "Centrap Park",
                            Description  = "Hello World"
                        }
                    }
                }
            };
        }
    }
}
