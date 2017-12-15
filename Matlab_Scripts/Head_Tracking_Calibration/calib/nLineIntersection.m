function intersection = nLineIntersection(origins, directions)
A = zeros(3, 3);
b = zeros(3, 1);

for i = 1:size(origins, 2)
   g = eye(3) - directions(1:3, i)*directions(1:3, i)';
   A = A + g; 
   b = b + g*origins(1:3, i);
end

intersection = A \ b;
end

