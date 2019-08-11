import cv2

source_image = "autumn_leaves.png"

image = cv2.imread(source_image, cv2.IMREAD_UNCHANGED)
width = len(image[0])
height = len(image)

# set alpha to 0% when there is black
for i in range(0, len(image)):
    for j in range(0, len(image[i])):
        if image[i][j][1] == 0 and image[i][j][0] == 0 and image[i][j][2] == 0:
            image[i][j][3] = 0


cv2.imwrite("autumn_leaves_.png", image)
