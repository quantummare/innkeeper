

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


Branch (filled customers, filled amenities, global minimum, global maximum, current minimum, current maximum, floor, amenity number, customer number)
    for each customer, calculates the score that customer would add to the minimum or remove from the maximum.
    Then, calculate a new global maximum based on the highest maximum of its children and do the same for minimum.
    Then, for each customer, run through: If customer's maximum < global minimum, ignore. Else, Branch(new filled customers, ..., ..., ..., current minimum, current maximum, floor, amenity number, customer number)
    
    
    
    if amenity number < 2:
        for each unassigned amenity:
            add this amenity to filled amenities
            Branch (..., new filled amenities, ..., ..., ..., amenity number + 1, ...)
    else if customer number = 2 AND floor = 4
        amenity1 = AmenityFloor[i][0]
        amenity2 = AmenityFloor[i][1]
        newscore = CustomerAmenity[customer number, amenity1] + CustomerAmenity[customer number, amenity2]
        end of permutation, return score + newscore
    else if customer number = 3
        for each unassigned customer:
            add this customer to filled customers
            add this floor 

*/