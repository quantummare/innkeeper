
namespace Innkeeper_Branch_and_Bound;

class InnkeeperBranchAndBound
{
    private static int CUSTOMER_COUNT = 14;
    private static int FLOOR_COUNT = 5;
    private static int AMENITY_COUNT = 12;
    private List<List<int>> CUSTOMER_AMENITY = 
        [[2, 2, 0, 0, 1, 0, 0, 1, 1, 2, 0, 0],  //native
         [2, 0, 2, 1, 1, 1, 0, 2, 0, 0, 0, 0],  //redneck
         [0, 0, 2, 0, 2, 2, 1, 0, 1, 0, 0, 1],  //bachelor
         [2, 1, 0, 0, 0, 0, 2, 2, 1, 0, 0, 1]]; //scientist
    
    int Branch(List<List<int>> customerFloor, List<List<int>> amenityFloor, int globalMinimum, int globalMaximum,
        int currentMinimum, int currentMaximum, int floor, int amenityNumber, int customerCount)
    {
        if (amenityNumber < 2)
        {
            int bestchild = 0;
            
            //since we're not calculating score in between amenities, we might as well do them both at once
            for (int i = 0; i < AMENITY_COUNT - 1; i++)
            {
                for (int j = i + 1; j < AMENITY_COUNT; j++)
                {
                    List<List<int>> newAmenityFloor = new List<List<int>>(amenityFloor);
                    newAmenityFloor[floor].Add(i);
                    newAmenityFloor[floor].Add(j);
                    
                    int pairscore = Branch(customerFloor, newAmenityFloor, globalMinimum, globalMaximum, currentMinimum, 
                        currentMaximum, floor, amenityNumber + 2, customerCount);

                    bestchild = int.Max(bestchild, pairscore);
                }
            }
            
            return bestchild;
        }
    
        else if (floor == FLOOR_COUNT)
        {
            int amenityindex1 = amenityFloor[floor][0];
            int amenityindex2 = amenityFloor[floor][1];

            int thisscore = 0;

            //there should be only two customers remaining
            for (int i = 0; i < CUSTOMER_COUNT; i++)
            {
                thisscore += CUSTOMER_AMENITY[i][amenityindex1] * customerFloor[i][floor] +
                             CUSTOMER_AMENITY[i][amenityindex2] * customerFloor[i][floor];
            }
            
            return thisscore;
        }
    
        else if (customerCount < 3)
        {
            //add all unassigned customers to a list with scores recorded
            var unassignedcustomers = new List<List<int>>();
            for (int i = 0; i < CUSTOMER_COUNT; i++)
            {
                if (customerFloor[FLOOR_COUNT][i] == 1)
                {
                    unassignedcustomers.Add([i, 0]);
                }
            }
            
            int amenityindex1 = amenityFloor[floor][0];
            int amenityindex2 = amenityFloor[floor][1];

            //calculate the scores for each customer
            foreach (var customer in unassignedcustomers)
            {
                customer[1] += CUSTOMER_AMENITY[customer[0]][amenityindex1] + CUSTOMER_AMENITY[customer[0]][amenityindex2];
            }
            
            //order the customers by score
            unassignedcustomers = unassignedcustomers.OrderBy(x => x[1]).ToList();

            int childrenBest = 0;
            
            
            foreach (var customer in unassignedcustomers)
            {
                int iscore = customer[1];
                int newmax = currentMaximum - (4 - iscore);
                int newmin = currentMinimum + iscore;

                if (newmax >= globalMinimum)
                {
                    List<List<int>> newCustomers = new List<List<int>>(customerFloor);
                    newCustomers[floor][customer[0]] = 1;
                    newCustomers[FLOOR_COUNT][customer[0]] = 0;

                    int childScore = Branch(newCustomers, amenityFloor, globalMinimum, globalMaximum,
                        newmin, newmax, floor, amenityNumber, customerCount + 1);
                    
                    childrenBest = int.Max(childrenBest, childScore);
                    globalMinimum = int.Max(globalMinimum, childrenBest);
                }
            }
            
            return childrenBest;
        }

        else            //only happens when customerCount == 3, so we should move to the next floor
        {
            return Branch(customerFloor, amenityFloor, globalMinimum, globalMaximum, currentMinimum, currentMaximum,
                floor + 1, 0, 0);
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
CUSTOMER_AMENITY = 14 * 12 array of scores {0, 1, 2} of which amenities are a customers favourite or secondary
Score = sum: for i in customerfloor, for j in amenityfloor, CUSTOMER_AMENITY(ij)

Maybe more like:
CustomerFloor = set of which floor each customer is on
AmenityFloor = set of which floor each amenity is on
CUSTOMER_AMENITY is the same
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
        cscore = CustomerAmentiy[customer amenity1] + CUSTOMER_AMENITY[customer amenity2]
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