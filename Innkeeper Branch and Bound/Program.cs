
namespace Innkeeper_Branch_and_Bound;

class InnkeeperBranchAndBound
{
    private List<List<int>> customerAmenity;
    
    int Branch(List<List<int>> filledCustomers, List<List<int>> filledAmenities, int globalMinimum, int globalMaximum,
        int currentMinimum, int currentMaximum, int floor, int amenityNumber, int customerNumber)
    {
        if (amenityNumber < 2)
        {
        
        }
    
        else if (customerNumber == 2 && floor == 4)
        {
        
        }
    
        else if (customerNumber < 3)
        {
            var newscores = new List<List<int>>();
            foreach (var customer in filledCustomers[5])
            {
                newscores.Add(new List<int> { customer, 0});
            }
            int amenity1 = filledAmenities[floor][0];
            int amenity2 = filledAmenities[floor][1];

            foreach (var customer in newscores)
            {
                customer[1] += customerAmenity[customer[0]][amenity1] + customerAmenity[customer[0]][amenity2];
            }
            
            newscores = newscores.OrderBy(x => x[1]).ToList();

            int childrenBest = 0;
            
            foreach (var customer in newscores)
            {
                int iscore = customer[1];
                int newmax = currentMaximum - (4 - iscore);
                int newmin = currentMinimum + iscore;

                if (newmax >= globalMaximum)
                {
                    List<List<int>> newCustomers = new List<List<int>>(filledCustomers);
                    newCustomers[floor].Add(customer[0]);
                }
            }
        }
    }
}


/*
14 Customer types
12 Amenities
5 Floors
Plan: 3 Customers (one floor just has two) and two amenities per floor

Scoring:

CustomerFloor = set of customers on floor i
AmenityFloor = set of Amenities on floor i
CustomerAmenity = 14 * 12 array of scores {0, 1, 2} of which amenities are a customers favourite or secondary
Score = sum: for i in customerfloor, for j in amenityfloor, customeramenity(ij)

Maybe more like:
CustomerFloor = set of which floor each customer is on
AmenityFloor = set of which floor each amenity is on
CustomerAmenity is the same
Score = sum: for i in 0-13, for j in 0-11, if CustomerFloor(i) == AmenityFloor(j), add CustomerAmentiy(ij) to Score

67e+6 permutations for customers
29e+6 for amenities
2e+15 combined total

clever ways to improve it must be found


branch and bound - how to decide minimum and maximum?

Amenities or Customers first?

We cannot know anything about the score until both amenities and customers are chosen.
Therefore, we will calculate permutations per floor of amenities, per customer.
24024 permutations at top floor

Start with both amenities chosen: 924 possibilities for first customer
Start with all three customers chosen: 4368 possibilities for first amenity
Therefore, start with amenities

Minimum: Assume other customers don't like either amenity, score + 0
Maximum: Assume other customers love both amenities, score + 8

theoretical maximum score = 56
Hand-found score: 51


filledCustomers is a list with 6 Lists within it. Each sublist has the indexes of all customers that are assigned to that floor

Branch (filled customers, filled amenities, global minimum, global maximum, current minimum, current maximum, floor, amenity number, customer number)
for each customer, calculates the score that customer would add to the minimum or remove from the maximum.
Then, calculate a new global maximum based on the highest maximum of its children and do the same for minimum.
Then, for each customer, run through: If customer's maximum < global minimum, ignore.
Otherwise, Branch(new filled customers, ..., ..., ..., current minimum, current maximum, floor, amenity number, customer number)

if amenity number < 2: for each unfilled amenity: Branch(..., new filled amenities, ..., ..., ..., ..., ..., amenity number + 1, ...)

else if customer number = 2 AND floor = 4:
    pick the final customer
    calculate their scores
    add that score to current minimum

else if customer number < 3:
    make list newscores (2 ints: score and customer index)
    amenity1 = filled amenities [floor][0]
    amenity2 = filled amenities [floor][1]
    for each unfilled customer:
        cscore = CustomerAmentiy[customer amenity1] + CustomerAmenity[customer amenity2]
        add cscore to newscores along with customer index
    sort newscores by score
    childrenbest = 0
    for i in newscores:
        iscore = newscores[i]
        newmax = current maximum - (4 - iscore)
        newmin = current minimum + iscore
        if newmax < global minimum, return 0
        else childscore = Branch (new filled customers, ..., ..., ..., newmin, newmax, floor, amenity number, customer number + 1)
            if childscore > childrenbest: childrenbest = childscore
            if childrenbest > globalminimum: globalminimum = childrenbest

*/