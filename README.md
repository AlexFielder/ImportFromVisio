# ImportFromVisio
 
IMPORT FROM VISIO


INTRODUCTION:
---------------------------------
This program uses information from a VSD file to create a lifecycle definition in Vault.


REQUIREMENTS:
---------------------------------
- Vault Workgroup 2010 or Vault Collaboration 2010 or Vault Manufacturing 2010 
- Microsoft Office Visio 2007


TO USE:
---------------------------------
1. Create your Visio file.  Use Process boxes for lifecycle states and Dynamic Connectors for state transitions.  The dynamic connectors can have double arrows. 
2. Save your diagram and close Visio. 
3. Run ImportFromVisio.exe. 
4. Log in as an administrator. 
5. Select the .VSD file.  Visio will automatically launch then immediately close.  This is normal.  
6. Fill in the rest of the data. 
7. Click the “Create Lifecycle Definition” button.  You should see an “Import Completed” message after a few seconds. 
8. Exit ImportFromVisio.exe 
9. Log into Vault as an administrator. 
10. Verify that your lifecycle definition exists with the proper states and transitions. 
11. Add in other data, such as security restrictions, transition criteria and associated categories. 


RESTRICTIONS ON VISIO FILE:
---------------------------------
- The lifecycle information must be on page one
- Process shapes should only be used to reprensent lifecycle states
- Dynamic Connector shapes should only be used to represent lifecycle transitions
- The Dynamic Connectors need to be anchored to the Process shapes.  Touching the shape is not enough.
- Anything that is not a Process or Dynamic Connector will be ignored by this application
- Do not use the same name for more than one state
- There must be a connection path between each state and each other state.  In other words, there should be no "islands" of states.


NOTES:
---------------------------------
- If you choose to save login information, that data will go into a file called settings.dat.  This file is unencrypted, so it is a potential security hole.