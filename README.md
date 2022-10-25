# Polygon drawer application

Application written with **.NET 6** using **Windows Forms**.

It allows for drawing polygons and adding relations and constraints for edges. User can also manage already drawn polygons, edges and vertices. The application implements the **Model-View-Presenter** architectural pattern.

## Description for user

Application works in four modes:

- **draw**: user can draw points by a left mouse click. At last user should end drawing a polygon by clicking at first drawn point. Drawing lines between points is done using library algorithm by default, but can use implemented *Bresenham Algorithm* in its basic version. To enable *Bresenham Algorithm* user should use a dedicated checkbox.

- **delete**: user can delete any vertex by left-clicking on it. This operation also removes all neighboring edges' constraints.

- **modify**: user can

    - move any vertex or edge by dragging them. Polygons also can be moved by dragging dedicated 'move icon' showing up when modify mode is chosen

    - insert point in the middle of edge using manage edge menu opened after right click on selected edge

    - set / unset fixed length of edge using manage edge menu opened after right click on selected edge

    - delete (perpendicular) relation of particular edge by right-click on one of relations appeared in the *Relations list* shown up after selecting particular edge (left click)

- **make perpendicular**: user can add perpendicular relation between two selected in this mode edges.

Added constraints are visible for better user experience:

- fixed lengths are displayed next to constrained edges

- perpendicular relations are marked with letters displayed next to edges

- perpendicular relations are also highlighted when user selects one of them from the *Relations list* shown up after selecting particular edge (left click).

Moreover, during item movement, (only directly) related to moving item edges are highlighted.

## Constraints algorithm description

To set and maintain constraints custom algorithm is used.
In a simplest possible way, when vertex or edge is being moved algorithm inspects polygon similarly to the *BFS algorithm* starting in moved item (one vertex if vertex is being moved, and two vertices if edge is being moved).
Then algorithm computes proper instructions to maintain constraints as a (pointToMove, vectorToMove) tuple for each reached vertex. Algorithm goes only through edges that has any constraint.
If algorithm must change some vertex second time it moves whole polygon.

It must be added that unfortunately in some corner cases algorithm are not perfectly working.

## Made assumptions

- user do not add any constraint that arithmetically can not be added in any way regarding already added constraints (e.g. add second perpendicular relation in triangles)

- user can add any perpendicular relation, but must be aware of that in some cases algorithm has malfunctions
