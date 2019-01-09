Some of the coding patterns used in the project:

1. Singleton		Pointers to instances of resource heavy dependencies 
			should be restricted to only one instance

2. Proxy 		Proxying access to database functions class

3. Facade		Holding references to various Data Access Layers and composing various objects into one 
			accessor

4. Factory		Using reflection to create instances of parent type classes via helper methods, 
			simplifies of calling similar constructors on child classes via just one method. 

5. Lazy instantiation	Allows delaying of creation of heavy objects. Heavy objects created when needed, 
			not when app starts. Cretes a smooth and much more responsive experience of using the app. 