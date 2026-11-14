
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
         [2, 1, 0, 0, 0, 0, 2, 2, 1, 0, 0, 1],  //pseudoscientist
         [0, 0, 2, 2, 0, 2, 1, 0, 0, 1, 1, 0],  //zoologist
         [0, 2, 0, 1, 0, 0, 0, 2, 1, 2, 1, 0],  //pensioner
         [1, 0, 1, 0, 2, 0, 0, 0, 2, 1, 2, 0],  //critic
         [1, 1, 0, 0, 2, 0, 2, 1, 0, 0, 0, 1],  //blogger
         [0, 0, 0, 0, 0, 2, 1, 0, 2, 1, 2, 1],  //socialite
         [0, 0, 1, 1, 0, 0, 0, 2, 0, 2, 2, 1],  //politician
         [1, 0, 2, 1, 0, 2, 0, 0, 0, 0, 1, 2],  //celebrity
         [0, 1, 1, 1, 0, 0, 2, 2, 0, 0, 0, 2],  //MIB
         [2, 1, 0, 2, 0, 1, 0, 0, 0, 2, 0, 1],  //hipster
         [0, 0, 1, 2, 0, 1, 0, 0, 2, 0, 1, 2]]; //jetsetter

    static void Main()
    {
        var b = new InnkeeperBranchAndBound();
        
    }

    void Solve()
    {
        int[] customerFloor = new int[CUSTOMER_COUNT];
        int[] amenityFloor = new int[AMENITY_COUNT];

        for (int i = 0; i < CUSTOMER_COUNT; i++)
        {
            customerFloor[i] = FLOOR_COUNT;
        }
        
        for (int i = 0; i < AMENITY_COUNT; i++)
        {
            amenityFloor[i] = FLOOR_COUNT;
        }
        
    }
    
    (int, int[], int[]) Branch(int[] customerFloor, int[] amenityFloor, int globalMinimum, int globalMaximum, 
        int currentMinimum, int currentMaximum, int floor, int amenityNumber, int customerCount)
    {
        if (amenityNumber < 2)
        {
            (int, int[], int[]) bestchild = (0, new int[CUSTOMER_COUNT], new int[AMENITY_COUNT]);
            
            //since we're not calculating score in between amenities, we might as well do them both at once
            for (int i = 0; i < AMENITY_COUNT - 1; i++)
            {
                if (amenityFloor[i] == FLOOR_COUNT)
                {
                    for (int j = i + 1; j < AMENITY_COUNT; j++)
                    {
                        if (amenityFloor[j] == FLOOR_COUNT)
                        {
                            int[] newAmenityFloor = new int[AMENITY_COUNT];
                            amenityFloor.CopyTo(newAmenityFloor, 0);
                            newAmenityFloor[i] = floor;
                            newAmenityFloor[j] = floor;
                    
                            (int, int[], int[]) thisscore = Branch(customerFloor, newAmenityFloor, globalMinimum, globalMaximum,
                                currentMinimum, currentMaximum, floor, amenityNumber + 2, customerCount);

                            if (thisscore.Item1 > bestchild.Item1)
                            {
                                bestchild = thisscore;

                                globalMinimum = int.Max(globalMinimum, thisscore.Item1);
                            }
                        }
                        
                    }
                }
                
            }
            
            return bestchild;
        }
    
        else if (floor == FLOOR_COUNT - 1)
        {
            int amenityindex1 = -1;
            int amenityindex2 = -1;

            for (int i = 0; i < AMENITY_COUNT; i++)
            {
                if (amenityFloor[i] == floor && amenityindex1 == -1)
                {
                    amenityindex1 = i;
                }
                else amenityindex2 = i;
            }

            int thisscore = 0;
            
            int[] newCustomerFloor = new int[CUSTOMER_COUNT];
            int[] newAmenityFloor = new int[AMENITY_COUNT];
            
            //there should be only two customers remaining
            for (int i = 0; i < CUSTOMER_COUNT; i++)
            {
                if (customerFloor[i] == FLOOR_COUNT)
                {
                    thisscore += CUSTOMER_AMENITY[i][amenityindex1] + CUSTOMER_AMENITY[i][amenityindex2];
                    newCustomerFloor[i] = floor;
                }
            }
            
            return (thisscore, newCustomerFloor, newAmenityFloor);
        }
    
        else if (customerCount < 3)
        {
            //add all unassigned customers to a list with scores recorded
            var unassignedcustomers = new List<List<int>>();
            for (int i = 0; i < CUSTOMER_COUNT; i++)
            {
                if (customerFloor[i] == FLOOR_COUNT)
                {
                    unassignedcustomers.Add([i, 0]);
                }
            }
            
            int amenityindex1 = -1;
            int amenityindex2 = -1;

            for (int i = 0; i < AMENITY_COUNT; i++)
            {
                if (amenityFloor[i] == floor && amenityindex1 == -1)
                {
                    amenityindex1 = i;
                }
                else amenityindex2 = i;
            }

            //calculate the scores for each customer
            foreach (var customer in unassignedcustomers)
            {
                customer[1] += CUSTOMER_AMENITY[customer[0]][amenityindex1] + CUSTOMER_AMENITY[customer[0]][amenityindex2];
            }
            
            //order the customers by score
            unassignedcustomers = unassignedcustomers.OrderBy(x => x[1]).ToList();

            (int, int[], int[]) bestchild = (0, new int[CUSTOMER_COUNT], new int[AMENITY_COUNT]);
            
            
            foreach (var customer in unassignedcustomers)
            {
                int iscore = customer[1];
                int newmax = currentMaximum - (4 - iscore);
                int newmin = currentMinimum + iscore;

                if (newmax >= globalMinimum)
                {
                    int[] newCustomerFloor = new int[CUSTOMER_COUNT];
                    customerFloor.CopyTo(newCustomerFloor, 0);
                    newCustomerFloor[customer[0]] = floor;

                    (int, int[], int[]) childScore = Branch(newCustomerFloor, amenityFloor, globalMinimum, globalMaximum,
                        newmin, newmax, floor, amenityNumber, customerCount = 1);

                    if (childScore.Item1 > bestchild.Item1)
                    {
                        bestchild = childScore;
                        globalMinimum = int.Max(globalMinimum, childScore.Item1);
                    }
                }
            }
            
            return bestchild;
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