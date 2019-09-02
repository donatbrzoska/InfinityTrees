import cv2
import random
import numpy as np

low_quality = "empty_low_res.png"
medium_quality = "empty_medium_res.png";
high_quality = "empty_high_res.png";

class LeafSize:
    def __init__(self, n_circles, min_radius, max_radius, max_diff_ratio_between, min_distance_to_others, suffix):
        self.n_circles = n_circles
        self.min_radius = min_radius
        self.max_radius = max_radius
        self.max_diff_ratio_between = max_diff_ratio_between
        self.min_distance_to_others = min_distance_to_others
        self.suffix = suffix

tiny = LeafSize(n_circles = 400, min_radius = 0.005, max_radius = 0.015, max_diff_ratio_between = 0.3, min_distance_to_others = 0.0018, suffix = "tiny")
small = LeafSize(n_circles = 250,
                 min_radius = 0.002,
                 max_radius = 0.04,
                 max_diff_ratio_between = 0.3,
                 min_distance_to_others = 0.03,
                 suffix = "small")
small_ = LeafSize(n_circles = 150,
                 min_radius = 0.002,
                 max_radius = 0.04,
                 max_diff_ratio_between = 0.3,
                 min_distance_to_others = 0.05,
                 suffix = "small")
medium = LeafSize(n_circles = 30,
                  min_radius = 0.04,
                  max_radius = 0.08,
                  max_diff_ratio_between = 0.3,
                  min_distance_to_others = 0.12,
                  suffix = "medium")
big = LeafSize(n_circles = 7,
               min_radius = 0.08,
               max_radius = 0.16,
               max_diff_ratio_between = 0.3,
               min_distance_to_others = 0.32,
               suffix = "big")
huge = LeafSize(n_circles = 2,
                min_radius = 0.16,
                max_radius = 0.32,
                max_diff_ratio_between = 0.3,
                min_distance_to_others = 0.5,
                suffix = "huge")


def position_okay(x, y, texture_space, leaf_offset_x, min_distance_to_others, positions):
    
    d2 = np.linalg.norm(np.array([x, y]) - np.array([leaf_offset_x + texture_space/2, texture_space/2]))
    if d2 > texture_space/2:
        return False

    for p in positions:
        
        d = np.linalg.norm(np.array([x,y])-p)
        if d < min_distance_to_others*texture_space:
            return False
    
    return True


def generate_particle(source_image,
                       stem_color,
                       stem_color_name,
                       leaf_color,
                       leaf_color_name,
                       n_circles,
                       min_radius,
                       max_radius,
                       max_diff_ratio_between,
                       min_distance_to_others,
                       suffix):
    image = cv2.imread(source_image, cv2.IMREAD_UNCHANGED)
    width = len(image[0])
    height = len(image)
    
    # indicates where the leaf texture begins in the image
    leaf_offset_x = int(len(image[0])/2)
    
    
    # STEM TEXTURE
    cv2.rectangle(image, (0,0), (leaf_offset_x-1, len(image)), stem_color, thickness=-1)
    
    
    # LEAF TEXTURE
    
    # indicates the total width and height available for the ellipses
    texture_space = leaf_offset_x-1
    
    positions = []
    
    random.seed(0)
    for i in range(0, n_circles):
        
        radius_y = int(random.uniform(min_radius, max_radius) * texture_space) + 1
        radius_x = int(random.uniform(min_radius, max_radius) * texture_space) + 1
        
        while (radius_y/radius_x < max_diff_ratio_between or radius_x/radius_y < max_diff_ratio_between):
            radius_y = int(random.uniform(min_radius, max_radius) * texture_space) + 1
            radius_x = int(random.uniform(min_radius, max_radius) * texture_space) + 1
        
        axes = (radius_x, radius_y)
        
        
        cx = random.randint(leaf_offset_x+radius_x, width-radius_x)
        cy = random.randint(radius_y, height-radius_y)
        while (not position_okay(cx, cy, texture_space, leaf_offset_x, min_distance_to_others, positions)):
            cx = random.randint(leaf_offset_x+radius_x, width-radius_x)
            cy = random.randint(radius_y, height-radius_y)
        center = (cx, cy)
        positions.append(np.array([cx, cy]))
        
        angle = 0
        start_angle = 0
        end_angle = 360
        cv2.ellipse(image, center, axes, angle, start_angle, end_angle, leaf_color, thickness=-1)
    
    
    
    # set alpha to 100% when there is a color
    for i in range(0, len(image)):
        for j in range(0, len(image[i])):
            if image[i][j][1] != 0 or image[i][j][0] != 0 or image[i][j][2] != 0:
                image[i][j][3] = 255


    cv2.imwrite("particle/" + stem_color_name + "_" + leaf_color_name + ("_" + suffix if (suffix != None) and (suffix != "") else "")   + ".png", image)



def generate_(source_image,
              stem_color,
              stem_color_name,
              leaf_color,
              leaf_color_name
              ):
    image = cv2.imread(source_image, cv2.IMREAD_UNCHANGED)
    width = len(image[0])
    height = len(image)
    
    # indicates where the leaf texture begins in the image
    leaf_offset_x = int(len(image[0])/2)
    
    
    # STEM TEXTURE
    cv2.rectangle(image, (0,0), (leaf_offset_x-1, height), stem_color, thickness=-1)
    
    # LEAF TEXTURE
    cv2.rectangle(image, (leaf_offset_x, 0), (width, height), leaf_color, thickness=-1)

    # set alpha to 100% when there is a color
    for i in range(0, len(image)):
        for j in range(0, len(image[i])):
            if image[i][j][1] != 0 or image[i][j][0] != 0 or image[i][j][2] != 0:
                image[i][j][3] = 255

    cv2.imwrite("triangle/" + stem_color_name + "_" + leaf_color_name + ".png", image)


def generate_simple(type, quality, stem_color, stem_color_name, leaf_color, leaf_color_name, leaf_size=None):
    if type=="particle":
        generate_particle(source_image = quality,
                 stem_color = stem_color,
                 stem_color_name = stem_color_name,
                 leaf_color = leaf_color,
                 leaf_color_name = leaf_color_name,
                 n_circles = leaf_size.n_circles,
                 min_radius = leaf_size.min_radius,
                 max_radius = leaf_size.max_radius,
                 max_diff_ratio_between = leaf_size.max_diff_ratio_between,
                 min_distance_to_others = leaf_size.min_distance_to_others,
                 suffix = leaf_size.suffix)
    elif type == "triangle":
        generate_(source_image=low_quality,
                  stem_color = stem_color,
                  stem_color_name = stem_color_name,
                  leaf_color = leaf_color,
                  leaf_color_name = leaf_color_name)


def generate_for_parameters(texture_type_list,
                            quality,
                            stem_color_list,
                            stem_color_name_list,
                            leaf_color_list,
                            leaf_color_name_list,
                            leaf_size_list):
    n_textures = len(texture_type_list) * len(stem_color_list) * len(leaf_color_list)
    n_done = 0;
    
    for i, texture_type in enumerate(texture_type_list):
        for j, stem_color in enumerate(stem_color_list):
            for k, leaf_color in enumerate(leaf_color_list):
                generate_simple(texture_type,
                                quality,
                                stem_color_list[j],
                                stem_color_name_list[j],
                                leaf_color_list[k],
                                leaf_color_name_list[k],
                                leaf_size_list[0])
                n_done += 1
                print("\rGenerating Textures "+str(int(100 * n_done / n_textures)) + "%", end='', flush=True)

stem_color_list = [(10, 31, 50), (52, 84, 122), (88, 165, 220), (92, 111, 122), (235, 241, 237)]
stem_color_name_list = ["dark_brown", "brown", "light_brown", "grey_brown", "greyish"]

leaf_color_list = [(30, 226, 255), (40, 98, 237), (46, 26, 198), (0, 255, 153), (18, 204, 92), (0, 177, 29), (24, 97, 0), (165, 165, 0), (94, 94, 0), (241, 211, 56)]
leaf_color_name_list = ["yellow", "orange", "red", "lime_green", "light_green", "green", "dark_green", "light_turquoise", "dark_turquoise", "blue"]

texture_type_list = ["triangle", "particle"]

small.suffix = ""
leaf_size_list = [small]


#generate_simple("triangle",
#                low_quality,
#                stem_color_list[2],
#                stem_color_name_list[2],
#                leaf_color_list[2],
#                leaf_color_name_list[2])

generate_for_parameters(texture_type_list,
                        medium_quality,
                        stem_color_list,
                        stem_color_name_list,
                        leaf_color_list,
                        leaf_color_name_list,
                        leaf_size_list)

#generate_for_parameters(texture_type_list,
#                        low_quality,
#                        stem_color_list,
#                        stem_color_name_list,
#                        leaf_color_list,
#                        leaf_color_name_list,
#                        leaf_size_list)

print()
