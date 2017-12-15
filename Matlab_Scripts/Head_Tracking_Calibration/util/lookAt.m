function M = lookAt(from, to, up)
%This function creates a view matrix that looks from from towards to with
%an up direction of up

if (size(from,2) > 1)
    from = from';
end
if (size(to,2) > 1)
    to = to';
end
if (size(up,2) > 1)
    up = up';
end

if (size(from,1) == 4)
    from = from(1:3);
end
if (size(to,1) == 4)
    to = to(1:3);
end
if (size(up,1) == 4)
    up = up(1:3);
end

%Calculate direction of local axes
forward = to - from;
forward = forward/norm(forward);
right = cross(up, forward);
right = right/norm(right);
up = cross(forward, right);
up = up/norm(up);

R = [right up forward]';
M = [[R -R*from]; 0 0 0 1];
end