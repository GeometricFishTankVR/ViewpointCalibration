function[X] = Process_Screens(I, win_size, nsx, nsy, num_screens, image_num)

    % Make the image grayscale and sharper for easier corner detection
    sharp_I = imsharpen(im2double(rgb2gray(I)));  
    sharp_I = sharp_I - min(sharp_I(:));
    sharp_I = sharp_I/max(sharp_I(:));
    
    % Show negation of image, makes it easier to see crosshairs.
    imshow(1 - sharp_I);
    title(['Image ' num2str(image_num)]);
    hold on
    
    % Find pixel location of the corners in each screen
    X = Find_Corners(I, win_size, nsx, nsy);
    for i = 2:num_screens
        X = cat(3,X,Find_Corners(I, win_size, nsx, nsy));
    end
    
    hold off
    %close
end