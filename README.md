# Notes:
In an ideal situation, an xUnit project would have been created from the onset of the project with all tests created before any code additions or modifications were made in order to follow a true TDD approach

# Challenge Notes

# Challenge 1:
  1. returned the built in http 400 codes as needed
  2. all returned data was converted to JSON
  3. JSON ignore attributes added where needed in order to ignore null
  4. StudentUtility contains the bulk of the logic (this was a practice we followed at work in order to separate heavy logic from the controller)

# Challenge 2:
  2. GPA is calculated in the StudentUtility
  
# Challenge 3:
  1. ALTER TABLE StudentGrade ADD CONSTRAINT Grade CHECK ( Grade >= 0 AND Grade <= 100)
  2. We can create a Unique Index on these two fields to maintain uniqueness
  
# Challenge 4:
  1-3: In Student Utility
  4. I assumed this meant that if a grade existed, we should not add a new grade. (would need clarification on this exact requirement)
