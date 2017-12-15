resolution = [1536 2048];
width = 8;
height = 6;
pix = resolution(2)/width;

im = ones(resolution(1), resolution(2));

for i = 1:resolution(1) 
    for j = 1:resolution(2)
        im(i,j) = xor(mod(floor((i-1)/pix),2),mod(floor((j-1)/pix),2));
    end
end

for i = 1:pix
    for j = 1:pix
        im(i,j) = (i-pix/2)^2 + (j-pix/2)^2 < (pix/3)^2;
    end
end
imshow(im)